// ------------------------------------------------------------------------------------------------
// <copyright file="InheritanceDiagramRenderer.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Reporting.Drawing
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using ECoreNetto.Extensions;
    using ECoreNetto.Reporting.Payload;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Microsoft.Msagl.Core.Geometry.Curves;
    using Microsoft.Msagl.Core.Layout;
    using Microsoft.Msagl.Core.Routing;
    using Microsoft.Msagl.Layout.Layered;

    using Svg;
    using Svg.DataTypes;
    using Svg.Pathing;

    /// <summary>
    /// The purpose of the <see cref="InheritanceDiagramRenderer"/> is to render an Ecore class diagram
    /// that shows the inheritance of the classes in an Ecore model.
    /// </summary>
    public class InheritanceDiagramRenderer : IInheritanceDiagramRenderer
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<InheritanceDiagramRenderer> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="InheritanceDiagramRenderer"/> class
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public InheritanceDiagramRenderer(ILoggerFactory? loggerFactory = null)
        {
            this.logger = loggerFactory == null ? NullLogger<InheritanceDiagramRenderer>.Instance : loggerFactory.CreateLogger<InheritanceDiagramRenderer>();
        }

        /// <summary>
        /// Renders a model-wide inheritance diagram for the classes in the provided <see cref="HandlebarsPayload"/>.
        /// </summary>
        /// <param name="payload">
        /// The <see cref="HandlebarsPayload"/> that contains the Ecore content.
        /// </param>
        /// <returns>
        /// a string that contains the diagram in SVG format.
        /// </returns>
        public string SvgRender(HandlebarsPayload payload)
        {
            var geometryGraph = this.GenerateGeometryGraphForClasses(payload.Classes.ToList());

            var svgDocument = this.GenerateSvg(geometryGraph);

            return Serialize(svgDocument);
        }

        /// <summary>
        /// Renders a per-class inheritance tree SVG diagram that highlights the target class and shows all
        /// its ancestors and descendants.
        /// </summary>
        /// <param name="targetClass">
        /// The <see cref="EClass"/> for which to render the inheritance tree.
        /// </param>
        /// <param name="payload">
        /// The <see cref="HandlebarsPayload"/> that contains the Ecore content.
        /// </param>
        /// <returns>
        /// a string that contains the per-class inheritance diagram in SVG format.
        /// </returns>
        public string SvgRenderForClass(EClass targetClass, HandlebarsPayload payload)
        {
            var ancestors = targetClass.QueryTypeHierarchy().ToList();
            var descendants = targetClass.QueryAllDescendantSpecializations(payload.Classes).ToList();

            var treeClasses = new HashSet<EClass>(ancestors) { targetClass };

            foreach (var descendant in descendants)
            {
                treeClasses.Add(descendant);
            }

            var filteredClasses = payload.Classes.Where(c => treeClasses.Contains(c)).ToList();

            var geometryGraph = this.GenerateGeometryGraphForClasses(filteredClasses);

            var svgDocument = this.GenerateSvgForClass(geometryGraph, targetClass);

            return Serialize(svgDocument);
        }

        /// <summary>
        /// Serializes an <see cref="SvgDocument"/> to an SVG string.
        /// </summary>
        /// <param name="svgDocument">
        /// The subject <see cref="SvgDocument"/>.
        /// </param>
        /// <returns>
        /// the SVG document as a string.
        /// </returns>
        private static string Serialize(SvgDocument svgDocument)
        {
            using var ms = new MemoryStream();
            svgDocument.Write(ms);
            ms.Position = 0;
            using var reader = new StreamReader(ms);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Generate a laid-out <see cref="GeometryGraph"/> for the provided classes, with an inheritance
        /// edge for each super type.
        /// </summary>
        /// <param name="classes">
        /// The classes to include in the graph.
        /// </param>
        /// <returns>
        /// an instance of <see cref="GeometryGraph"/>.
        /// </returns>
        private GeometryGraph GenerateGeometryGraphForClasses(List<EClass> classes)
        {
            var geometryGraph = new GeometryGraph();

            foreach (var @class in classes)
            {
                var (height, width) = SvgDrawingHelper.EstimateBoxSize(@class.Name);

                var curve = CurveFactory.CreateRectangle(width, height, new Microsoft.Msagl.Core.Geometry.Point());

                var node = new Node(curve, @class);

                geometryGraph.Nodes.Add(node);
            }

            foreach (var @class in classes)
            {
                foreach (var superClass in @class.ESuperTypes)
                {
                    var sourceNode = geometryGraph.FindNodeByUserData(@class);
                    var targetNode = geometryGraph.FindNodeByUserData(superClass);

                    if (sourceNode != null && targetNode != null)
                    {
                        var edge = new Edge(sourceNode, targetNode);

                        geometryGraph.Edges.Add(edge);
                    }
                }
            }

            this.logger.LogInformation("inheritance edges count: {Edges}", geometryGraph.Edges.Count);

            var settings = new SugiyamaLayoutSettings
            {
                LayerSeparation = 30,
                NodeSeparation = 10,
                EdgeRoutingSettings = new EdgeRoutingSettings
                {
                    EdgeRoutingMode = EdgeRoutingMode.Rectilinear,
                },
            };

            var layoutEngine = new LayeredLayout(geometryGraph, settings);
            layoutEngine.Run();

            return geometryGraph;
        }

        /// <summary>
        /// Generates an SVG document for the whole-model inheritance diagram.
        /// </summary>
        /// <param name="geometryGraph">
        /// the subject <see cref="GeometryGraph"/>.
        /// </param>
        /// <returns>
        /// The generated <see cref="SvgDocument"/>.
        /// </returns>
        private SvgDocument GenerateSvg(GeometryGraph geometryGraph)
        {
            const float padding = 10f;

            var bbox = geometryGraph.BoundingBox;

            var width = (float)(bbox.Width + 2 * padding);
            var height = (float)(bbox.Height + 2 * padding);

            var svgDocument = new SvgDocument
            {
                Width = width,
                Height = height,
                ViewBox = new SvgViewBox(
                    (float)(bbox.Left - padding),
                    (float)(bbox.Bottom - padding),
                    width,
                    height),
                ID = "inheritance-diagram"
            };

            svgDocument.Children.Add(new SvgTitle { Content = "Ecore Inheritance Diagram" });
            svgDocument.Children.Add(new SvgDescription
            {
                Content = "An Ecore diagram showing class inheritance relationships - generated with EcoreNetto"
            });

            svgDocument.Children.Add(CreateBorder(bbox, padding, width, height));

            svgDocument.Children.Add(CreateArrowMarker("generalization-arrow"));

            foreach (var node in geometryGraph.Nodes)
            {
                svgDocument.Children.Add(this.ConvertNodeToRectangleAndLabel(node, false));
            }

            foreach (var edge in geometryGraph.Edges)
            {
                var svgPath = this.ConvertEdgeToSvgPath(edge, "generalization-arrow");
                if (svgPath != null)
                {
                    svgDocument.Children.Add(svgPath);
                }
            }

            return svgDocument;
        }

        /// <summary>
        /// Generates an SVG document for a per-class inheritance tree with the target class highlighted.
        /// </summary>
        /// <param name="geometryGraph">
        /// the subject <see cref="GeometryGraph"/>.
        /// </param>
        /// <param name="targetClass">
        /// The <see cref="EClass"/> that should be highlighted in the diagram.
        /// </param>
        /// <returns>
        /// The generated <see cref="SvgDocument"/>.
        /// </returns>
        private SvgDocument GenerateSvgForClass(GeometryGraph geometryGraph, EClass targetClass)
        {
            const float padding = 10f;

            var bbox = geometryGraph.BoundingBox;

            var width = (float)(bbox.Width + 2 * padding);
            var height = (float)(bbox.Height + 2 * padding);

            var anchor = targetClass.QueryAnchorId();
            var markerId = $"gen-arrow-{anchor}";

            var svgDocument = new SvgDocument
            {
                Width = width,
                Height = height,
                ViewBox = new SvgViewBox(
                    (float)(bbox.Left - padding),
                    (float)(bbox.Bottom - padding),
                    width,
                    height),
                ID = $"inheritance-tree-{anchor}"
            };

            svgDocument.Children.Add(CreateBorder(bbox, padding, width, height));
            svgDocument.Children.Add(CreateArrowMarker(markerId));

            foreach (var node in geometryGraph.Nodes)
            {
                var @class = (EClass)node.UserData;
                svgDocument.Children.Add(this.ConvertNodeToRectangleAndLabel(node, @class == targetClass));
            }

            foreach (var edge in geometryGraph.Edges)
            {
                var svgPath = this.ConvertEdgeToSvgPath(edge, markerId);
                if (svgPath != null)
                {
                    svgDocument.Children.Add(svgPath);
                }
            }

            return svgDocument;
        }

        /// <summary>
        /// Creates the diagram border rectangle.
        /// </summary>
        /// <param name="bbox">the bounding box of the graph.</param>
        /// <param name="padding">the diagram padding.</param>
        /// <param name="width">the diagram width.</param>
        /// <param name="height">the diagram height.</param>
        /// <returns>the border <see cref="SvgRectangle"/>.</returns>
        private static SvgRectangle CreateBorder(Microsoft.Msagl.Core.Geometry.Rectangle bbox, float padding, float width, float height)
        {
            return new SvgRectangle
            {
                X = (float)(bbox.Left - padding),
                Y = (float)(bbox.Bottom - padding),
                Width = width,
                Height = height,
                Fill = SvgPaintServer.None,
                Stroke = new SvgColourServer(System.Drawing.Color.Black),
                StrokeWidth = 1
            };
        }

        /// <summary>
        /// Creates a hollow-triangle generalization arrow marker with the provided id.
        /// </summary>
        /// <param name="markerId">the marker id.</param>
        /// <returns>the <see cref="SvgMarker"/>.</returns>
        private static SvgMarker CreateArrowMarker(string markerId)
        {
            var arrowMarker = new SvgMarker
            {
                ID = markerId,
                MarkerUnits = SvgMarkerUnits.StrokeWidth,
                MarkerWidth = 10,
                MarkerHeight = 10,
                RefX = 10,
                RefY = 5,
                Orient = new SvgOrient { IsAuto = true }
            };

            arrowMarker.Children.Add(new SvgPath
            {
                Stroke = new SvgColourServer(System.Drawing.Color.Black),
                Fill = new SvgColourServer(System.Drawing.Color.White),
                StrokeWidth = 1,
                PathData = SvgPathBuilder.Parse("M0,0 L10,5 L0,10 Z".AsSpan())
            });

            return arrowMarker;
        }

        /// <summary>
        /// Converts a <see cref="Node"/> to an <see cref="SvgGroup"/> containing a rectangle, label and tooltip.
        /// </summary>
        /// <param name="node">
        /// The <see cref="Node"/> that represents an <see cref="EClass"/> in the inheritance diagram.
        /// </param>
        /// <param name="isTarget">
        /// Whether this node represents the target class that should be highlighted.
        /// </param>
        /// <returns>
        /// the <see cref="SvgGroup"/>.
        /// </returns>
        private SvgGroup ConvertNodeToRectangleAndLabel(Node node, bool isTarget)
        {
            var box = node.BoundingBox;
            var @class = (EClass)node.UserData;

            var fillColor = isTarget ? System.Drawing.Color.FromArgb(5, 166, 229) : System.Drawing.Color.White;
            var textColor = isTarget ? System.Drawing.Color.White : System.Drawing.Color.Black;

            var anchor = new SvgAnchor
            {
                Href = $"#{@class.QueryAnchorId()}"
            };

            var rectangle = new SvgRectangle
            {
                X = (float)box.Left,
                Y = (float)box.Bottom,
                Width = (float)box.Width,
                Height = (float)box.Height,
                Fill = new SvgColourServer(fillColor),
                Stroke = new SvgColourServer(System.Drawing.Color.Black)
            };

            var label = new SvgText(@class.Name)
            {
                X = { (float)box.Center.X },
                Y = { (float)box.Center.Y + 4 },
                TextAnchor = SvgTextAnchor.Middle,
                FontSize = 12,
                FontFamily = "sans-serif",
                Fill = new SvgColourServer(textColor),
                FontStyle = @class.Abstract ? SvgFontStyle.Italic : SvgFontStyle.Normal
            };

            var tooltipText = $"Name: {@class.Name}\n" +
                              $"Is Abstract: {@class.Abstract}\n" +
                              $"Superclasses: {string.Join(", ", @class.ESuperTypes.Select(c => c.Name))}\n" +
                              $"Description: {@class.QueryRawDocumentation()}";

            anchor.Children.Add(rectangle);
            anchor.Children.Add(label);
            anchor.Children.Add(new SvgTitle { Content = tooltipText });

            var group = new SvgGroup();
            group.Children.Add(anchor);

            return group;
        }

        /// <summary>
        /// Converts an <see cref="Edge"/> into an <see cref="SvgPath"/> using the specified marker id.
        /// </summary>
        /// <param name="edge">
        /// The subject <see cref="Edge"/> that is to be converted.
        /// </param>
        /// <param name="markerId">
        /// The id of the arrow marker to use.
        /// </param>
        /// <returns>
        /// the resulting <see cref="SvgPath"/>, or null when the edge has no curve.
        /// </returns>
        private SvgPath? ConvertEdgeToSvgPath(Edge edge, string markerId)
        {
            var curve = edge.Curve;
            if (curve == null)
            {
                return null;
            }

            var segments = new SvgPathSegmentList();

            segments.Add(new SvgMoveToSegment(false, SvgDrawingHelper.ToPointF(curve.Start)));

            switch (curve)
            {
                case Curve compound:
                    foreach (var segment in compound.Segments)
                    {
                        this.AddSegment(segments, segment);
                    }

                    break;

                case LineSegment line:
                    segments.Add(new SvgLineSegment(false, SvgDrawingHelper.ToPointF(line.End)));
                    break;

                case CubicBezierSegment bezier:
                    segments.Add(new SvgCubicCurveSegment(
                        false,
                        SvgDrawingHelper.ToPointF(bezier.B(0)),
                        SvgDrawingHelper.ToPointF(bezier.B(1)),
                        SvgDrawingHelper.ToPointF(bezier.B(3))));
                    break;

                case Polyline polyline:
                    foreach (var point in polyline.Skip(1))
                    {
                        segments.Add(new SvgLineSegment(false, SvgDrawingHelper.ToPointF(point)));
                    }

                    break;

                default:
                    this.logger.LogWarning("Unsupported Curve type encountered: {CurveType}", curve.GetType().FullName);
                    return null;
            }

            return new SvgPath
            {
                Stroke = new SvgColourServer(System.Drawing.Color.Black),
                Fill = SvgPaintServer.None,
                PathData = segments,
                MarkerEnd = new Uri($"url(#{markerId})", UriKind.Relative)
            };
        }

        /// <summary>
        /// Adds a segment to the given <see cref="SvgPathSegmentList"/> based on the specified MSAGL curve segment.
        /// </summary>
        /// <param name="segments">
        /// The <see cref="SvgPathSegmentList"/> to which the SVG path segment will be added.
        /// </param>
        /// <param name="segment">
        /// The MSAGL <see cref="ICurve"/> segment to convert into an SVG path segment.
        /// </param>
        private void AddSegment(SvgPathSegmentList segments, ICurve segment)
        {
            switch (segment)
            {
                case LineSegment line:
                    segments.Add(new SvgLineSegment(false, SvgDrawingHelper.ToPointF(line.End)));
                    break;

                case CubicBezierSegment bezier:
                    segments.Add(new SvgCubicCurveSegment(
                        false,
                        SvgDrawingHelper.ToPointF(bezier.B(0)),
                        SvgDrawingHelper.ToPointF(bezier.B(1)),
                        SvgDrawingHelper.ToPointF(bezier.B(3))));
                    break;

                default:
                    this.logger.LogWarning("Unsupported segment type encountered: {SegmentType}", segment.GetType().FullName);
                    break;
            }
        }
    }
}
