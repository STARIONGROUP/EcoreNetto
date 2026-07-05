// ------------------------------------------------------------------------------------------------
// <copyright file="AssociationDiagramRenderer.cs" company="Starion Group S.A.">
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
    /// The purpose of the <see cref="AssociationDiagramRenderer"/> is to render an Ecore association diagram
    /// that shows a class and all its first-order neighbours connected by typed references.
    /// </summary>
    public class AssociationDiagramRenderer : IAssociationDiagramRenderer
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<AssociationDiagramRenderer> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssociationDiagramRenderer"/> class
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public AssociationDiagramRenderer(ILoggerFactory? loggerFactory = null)
        {
            this.logger = loggerFactory == null ? NullLogger<AssociationDiagramRenderer>.Instance : loggerFactory.CreateLogger<AssociationDiagramRenderer>();
        }

        /// <summary>
        /// Renders a per-class association SVG diagram that shows the target class and all classes connected
        /// to it via typed references.
        /// </summary>
        /// <param name="targetClass">
        /// The <see cref="EClass"/> for which to render the association diagram.
        /// </param>
        /// <param name="payload">
        /// The <see cref="HandlebarsPayload"/> that contains the Ecore content.
        /// </param>
        /// <returns>
        /// a string that contains the per-class association diagram in SVG format, or an empty string when
        /// the class has no associations.
        /// </returns>
        public string SvgRenderForClass(EClass targetClass, HandlebarsPayload payload)
        {
            var classSet = new HashSet<EClass>(payload.Classes);

            var associationEdges = CollectAssociationEdges(targetClass, payload.Classes, classSet);

            if (associationEdges.Count == 0)
            {
                return string.Empty;
            }

            var involvedClasses = new HashSet<EClass> { targetClass };

            foreach (var edge in associationEdges)
            {
                involvedClasses.Add(edge.OwnerClass);
                involvedClasses.Add(edge.TypeClass);
            }

            var geometryGraph = this.GenerateGeometryGraph(involvedClasses.ToList(), associationEdges);

            var svgDocument = this.GenerateSvg(geometryGraph, targetClass);

            using var ms = new MemoryStream();
            svgDocument.Write(ms);
            ms.Position = 0;
            using var reader = new StreamReader(ms);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Collects all association edges (reference-based relationships) for the target class.
        /// </summary>
        /// <param name="targetClass">
        /// The <see cref="EClass"/> for which to collect associations.
        /// </param>
        /// <param name="allClasses">
        /// All classes in the payload.
        /// </param>
        /// <param name="classSet">
        /// A <see cref="HashSet{T}"/> of all classes for fast lookup.
        /// </param>
        /// <returns>
        /// A list of <see cref="AssociationEdgeInfo"/> representing all associations.
        /// </returns>
        private static List<AssociationEdgeInfo> CollectAssociationEdges(EClass targetClass, IEnumerable<EClass> allClasses, HashSet<EClass> classSet)
        {
            var edges = new List<AssociationEdgeInfo>();

            foreach (var reference in targetClass.EStructuralFeatures.OfType<EReference>())
            {
                if (reference.EType is EClass typeClass && classSet.Contains(typeClass))
                {
                    edges.Add(new AssociationEdgeInfo(targetClass, typeClass, reference));
                }
            }

            foreach (var otherClass in allClasses)
            {
                if (otherClass == targetClass)
                {
                    continue;
                }

                foreach (var reference in otherClass.EStructuralFeatures.OfType<EReference>())
                {
                    if (reference.EType is EClass typeClass && typeClass == targetClass)
                    {
                        edges.Add(new AssociationEdgeInfo(otherClass, targetClass, reference));
                    }
                }
            }

            return edges;
        }

        /// <summary>
        /// Generates a laid-out <see cref="GeometryGraph"/> for the association diagram.
        /// </summary>
        /// <param name="classes">
        /// The classes to include in the graph.
        /// </param>
        /// <param name="associationEdges">
        /// The association edges to include.
        /// </param>
        /// <returns>
        /// an instance of <see cref="GeometryGraph"/>.
        /// </returns>
        private GeometryGraph GenerateGeometryGraph(List<EClass> classes, List<AssociationEdgeInfo> associationEdges)
        {
            var geometryGraph = new GeometryGraph();

            foreach (var @class in classes)
            {
                var (height, width) = SvgDrawingHelper.EstimateBoxSize(@class.Name);

                var curve = CurveFactory.CreateRectangle(width, height, new Microsoft.Msagl.Core.Geometry.Point());

                var node = new Node(curve, @class);

                geometryGraph.Nodes.Add(node);
            }

            foreach (var assocEdge in associationEdges)
            {
                var sourceNode = geometryGraph.FindNodeByUserData(assocEdge.OwnerClass);
                var targetNode = geometryGraph.FindNodeByUserData(assocEdge.TypeClass);

                if (sourceNode != null && targetNode != null)
                {
                    var edge = new Edge(sourceNode, targetNode)
                    {
                        UserData = assocEdge
                    };

                    geometryGraph.Edges.Add(edge);
                }
            }

            var settings = new SugiyamaLayoutSettings
            {
                LayerSeparation = 180,
                NodeSeparation = 120,
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
        /// Generates an SVG document for the association diagram.
        /// </summary>
        /// <param name="geometryGraph">
        /// The subject <see cref="GeometryGraph"/>.
        /// </param>
        /// <param name="targetClass">
        /// The <see cref="EClass"/> that should be highlighted in the diagram.
        /// </param>
        /// <returns>
        /// The generated <see cref="SvgDocument"/>.
        /// </returns>
        private SvgDocument GenerateSvg(GeometryGraph geometryGraph, EClass targetClass)
        {
            const float padding = 40f;

            var bbox = geometryGraph.BoundingBox;

            var width = (float)(bbox.Width + 2 * padding);
            var height = (float)(bbox.Height + 2 * padding);

            var anchor = targetClass.QueryAnchorId();

            var svgDocument = new SvgDocument
            {
                Width = width,
                Height = height,
                ViewBox = new SvgViewBox(
                    (float)(bbox.Left - padding),
                    (float)(bbox.Bottom - padding),
                    width,
                    height),
                ID = $"association-diagram-{anchor}"
            };

            svgDocument.Children.Add(new SvgRectangle
            {
                X = (float)(bbox.Left - padding),
                Y = (float)(bbox.Bottom - padding),
                Width = width,
                Height = height,
                Fill = SvgPaintServer.None,
                Stroke = new SvgColourServer(System.Drawing.Color.Black),
                StrokeWidth = 1
            });

            this.AddMarkerDefinitions(svgDocument, anchor);

            foreach (var node in geometryGraph.Nodes)
            {
                var @class = (EClass)node.UserData;
                svgDocument.Children.Add(this.ConvertNodeToRectangleAndLabel(node, @class == targetClass));
            }

            foreach (var edge in geometryGraph.Edges)
            {
                var group = this.ConvertEdgeToSvgGroup(edge, anchor);
                if (group != null)
                {
                    svgDocument.Children.Add(group);
                }
            }

            return svgDocument;
        }

        /// <summary>
        /// Adds SVG marker definitions for the composition diamond and the navigability arrow.
        /// </summary>
        /// <param name="svgDocument">
        /// The <see cref="SvgDocument"/> to add markers to.
        /// </param>
        /// <param name="idPrefix">
        /// A prefix for marker ids to ensure uniqueness.
        /// </param>
        private void AddMarkerDefinitions(SvgDocument svgDocument, string idPrefix)
        {
            var compositionMarker = new SvgMarker
            {
                ID = $"composition-diamond-{idPrefix}",
                MarkerUnits = SvgMarkerUnits.StrokeWidth,
                MarkerWidth = 12,
                MarkerHeight = 8,
                RefX = 0,
                RefY = 4,
                Orient = new SvgOrient { IsAuto = true }
            };

            compositionMarker.Children.Add(new SvgPath
            {
                Stroke = new SvgColourServer(System.Drawing.Color.Black),
                Fill = new SvgColourServer(System.Drawing.Color.Black),
                StrokeWidth = 1,
                PathData = SvgPathBuilder.Parse("M0,4 L6,0 L12,4 L6,8 Z".AsSpan())
            });

            svgDocument.Children.Add(compositionMarker);

            var arrowMarker = new SvgMarker
            {
                ID = $"navigable-arrow-{idPrefix}",
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
                Fill = SvgPaintServer.None,
                StrokeWidth = 1,
                PathData = SvgPathBuilder.Parse("M0,0 L10,5 L0,10".AsSpan())
            });

            svgDocument.Children.Add(arrowMarker);
        }

        /// <summary>
        /// Converts a <see cref="Node"/> to an <see cref="SvgGroup"/> containing a rectangle, label and tooltip.
        /// </summary>
        /// <param name="node">
        /// The <see cref="Node"/> that represents an <see cref="EClass"/> in the association diagram.
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

            anchor.Children.Add(rectangle);
            anchor.Children.Add(label);
            anchor.Children.Add(new SvgTitle { Content = $"Name: {@class.Name}\nIs Abstract: {@class.Abstract}" });

            var group = new SvgGroup();
            group.Children.Add(anchor);

            return group;
        }

        /// <summary>
        /// Converts an <see cref="Edge"/> into an <see cref="SvgGroup"/> containing the edge path, markers,
        /// multiplicity labels and role name.
        /// </summary>
        /// <param name="edge">
        /// The subject <see cref="Edge"/> that is to be converted.
        /// </param>
        /// <param name="idPrefix">
        /// A prefix for marker reference ids.
        /// </param>
        /// <returns>
        /// the resulting <see cref="SvgGroup"/>, or null when the edge has no curve.
        /// </returns>
        private SvgGroup? ConvertEdgeToSvgGroup(Edge edge, string idPrefix)
        {
            var curve = edge.Curve;
            if (curve == null)
            {
                return null;
            }

            var assocEdge = (AssociationEdgeInfo)edge.UserData;
            var reference = assocEdge.Reference;

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

            var svgPath = new SvgPath
            {
                Stroke = new SvgColourServer(System.Drawing.Color.Black),
                Fill = SvgPaintServer.None,
                PathData = segments,
                MarkerEnd = new Uri($"url(#navigable-arrow-{idPrefix})", UriKind.Relative)
            };

            if (reference.IsContainment)
            {
                svgPath.MarkerStart = new Uri($"url(#composition-diamond-{idPrefix})", UriKind.Relative);
            }

            var group = new SvgGroup();

            var hitArea = new SvgPath
            {
                Stroke = new SvgColourServer(System.Drawing.Color.Transparent),
                Fill = SvgPaintServer.None,
                PathData = segments,
                StrokeWidth = 12
            };

            group.Children.Add(hitArea);
            group.Children.Add(svgPath);

            var multiplicity = FormatMultiplicity(reference.LowerBound, reference.UpperBound);

            group.Children.Add(new SvgTitle
            {
                Content = $"Source: {assocEdge.OwnerClass.Name}\n" +
                          $"Target: {assocEdge.TypeClass.Name}\n" +
                          $"Reference: {reference.Name}\n" +
                          $"Multiplicity: {multiplicity}\n" +
                          $"Containment: {reference.IsContainment}"
            });

            var endPoint = curve.End;
            var penultimatePoint = GetPenultimatePoint(curve);
            var (labelOffsetX, labelOffsetY, textAnchor) = ComputeLabelOffset(endPoint, penultimatePoint);

            group.Children.Add(new SvgText(multiplicity)
            {
                X = { (float)(endPoint.X + labelOffsetX) },
                Y = { (float)(endPoint.Y + labelOffsetY - 10) },
                TextAnchor = textAnchor,
                FontSize = 10,
                FontFamily = "sans-serif",
                Fill = new SvgColourServer(System.Drawing.Color.DarkBlue)
            });

            if (!string.IsNullOrEmpty(reference.Name))
            {
                group.Children.Add(new SvgText(reference.Name)
                {
                    X = { (float)(endPoint.X + labelOffsetX) },
                    Y = { (float)(endPoint.Y + labelOffsetY + 4) },
                    TextAnchor = textAnchor,
                    FontSize = 10,
                    FontFamily = "sans-serif",
                    Fill = new SvgColourServer(System.Drawing.Color.DarkGray)
                });
            }

            return group;
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

        /// <summary>
        /// Gets the penultimate point on the curve (the point just before the end), used to determine the
        /// direction the edge approaches its target.
        /// </summary>
        /// <param name="curve">
        /// The <see cref="ICurve"/> from which to extract the penultimate point.
        /// </param>
        /// <returns>
        /// The penultimate <see cref="Microsoft.Msagl.Core.Geometry.Point"/>.
        /// </returns>
        private static Microsoft.Msagl.Core.Geometry.Point GetPenultimatePoint(ICurve curve)
        {
            switch (curve)
            {
                case Curve compound when compound.Segments.Count > 0:
                    return compound.Segments[compound.Segments.Count - 1].Start;

                case Polyline polyline:
                    var points = polyline.ToArray();
                    return points.Length >= 2 ? points[points.Length - 2] : curve.Start;

                default:
                    return curve.Start;
            }
        }

        /// <summary>
        /// Computes the label offset and text anchor based on the direction the edge approaches the target node.
        /// </summary>
        /// <param name="endPoint">
        /// The endpoint of the edge curve.
        /// </param>
        /// <param name="penultimatePoint">
        /// The point just before the endpoint on the curve.
        /// </param>
        /// <returns>
        /// A tuple of (offsetX, offsetY, textAnchor) for positioning the label.
        /// </returns>
        private static (double OffsetX, double OffsetY, SvgTextAnchor Anchor) ComputeLabelOffset(
            Microsoft.Msagl.Core.Geometry.Point endPoint,
            Microsoft.Msagl.Core.Geometry.Point penultimatePoint)
        {
            var dx = endPoint.X - penultimatePoint.X;
            var dy = endPoint.Y - penultimatePoint.Y;

            if (Math.Abs(dx) > Math.Abs(dy))
            {
                if (dx > 0)
                {
                    return (-20, -14, SvgTextAnchor.End);
                }

                return (20, -14, SvgTextAnchor.Start);
            }

            if (dy > 0)
            {
                return (10, -24, SvgTextAnchor.Start);
            }

            return (10, 28, SvgTextAnchor.Start);
        }

        /// <summary>
        /// Formats the multiplicity of a reference as <c>[lower..upper]</c>, using <c>*</c> for the unbounded
        /// upper bound (-1).
        /// </summary>
        /// <param name="lowerBound">the lower bound.</param>
        /// <param name="upperBound">the upper bound (-1 means unbounded).</param>
        /// <returns>the formatted multiplicity string.</returns>
        private static string FormatMultiplicity(int lowerBound, int upperBound)
        {
            var upper = upperBound == -1 ? "*" : upperBound.ToString();

            return $"[{lowerBound}..{upper}]";
        }

        /// <summary>
        /// Represents an association edge between two classes via a reference.
        /// </summary>
        private class AssociationEdgeInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AssociationEdgeInfo"/> class.
            /// </summary>
            /// <param name="ownerClass">The class that owns the reference.</param>
            /// <param name="typeClass">The class that is the type of the reference.</param>
            /// <param name="reference">The reference that defines the relationship.</param>
            public AssociationEdgeInfo(EClass ownerClass, EClass typeClass, EReference reference)
            {
                this.OwnerClass = ownerClass;
                this.TypeClass = typeClass;
                this.Reference = reference;
            }

            /// <summary>
            /// Gets the class that owns the reference.
            /// </summary>
            public EClass OwnerClass { get; }

            /// <summary>
            /// Gets the class that is the type of the reference.
            /// </summary>
            public EClass TypeClass { get; }

            /// <summary>
            /// Gets the reference that defines the relationship.
            /// </summary>
            public EReference Reference { get; }
        }
    }
}
