# CurvePlus
A Grasshopper 3d plugin adding a grab bag of curve utilities.


### Grids
The Grids available in default Grasshopper are extended with new Radial Grids
> *Tab : Vector* | *Panel : Grid*
 - Radial Diamond Grid
 - Radial Hexagon Grid
 - Radial Quad Grid
 - Radial Triangle Grid
 - Radial Triangle Grid B

### Analysis
Additional Curve Properties and Elements are exposed from Rhino Common
> *Tab: Curve* | *Panel: Analysis*
 - Curve Spans
 - Greyville Points

### Bezier
Rhinocommon features several Bezier curve classes and methods which are exposed
> *Tab: Curve* | *Panel: Bezier*
 - Curve To Bezier Spline
 - Loft Bezier
 - To Bezier Spans
 - To Cubic Bezier Spans

### Utilities & Modifiers
There are a large number of curve creation and modifier components added which are open new functionality or expose existing methods.
> *Tab: Curve* | *Panel: Util*
 - Snub Polyline
 - Smooth Corners
 - Smooth Corners By Distance
 - Loft Bezier
 - Tri Fan Polyline
 - Quad Fan Polyline
 - MidEdge Polyline
 - Triangulate Closed Polyline
 - Close Curve
 - Offset by Parameter
 - Remove Points
 - Remove Segments
 - Weight Control Points

### Polygons
Several new polygon creation methods for regular polygons and rectangles.
> *Tab: Curve* | *Panel: Primative*
 - Circumscribed Polygon
 - Inscribed Polygon
 - Polygon by Edge Length
 - Star Polygon
 - Bounding Rectangle

### Splines
Rhinocommon features several Nurbs curve classes which are exposed
> *Tab: Curve* | *Panel: Spline*
 - Spiral
 - Spiral Rail

### Voronoi
Implementations of the [Lloyds algorithm](https://en.wikipedia.org/wiki/Lloyd%27s_algorithm/) are included in the mesh tab
> *Tab: Mesh* | *Panel: Triangulation*
 - Lloyds Algorithm
 - Lloyds Algorithm Constrained

## Dependencies
 - [Rhinoceros 3d](https://www.rhino3d.com/)
 - [Rhinocommon](https://www.nuget.org/packages/RhinoCommon/5.12.50810.13095)
