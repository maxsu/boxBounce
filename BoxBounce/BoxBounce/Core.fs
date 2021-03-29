module BoxBounce.Core

open System
open System.Drawing
open Rhino
open Rhino.Input
open Rhino.Geometry
open Rhino.Geometry.Intersect.Intersection
open Rhino.Commands
open Microsoft.FSharp.Collections
open GosLib
open Rhino.Display
module rs = GosLib.RhinoScriptSyntax

let rnd = new Random()
let unitize vector =
    let vector' = vector.copy
    vector'.Unitize() |> ignore
    vector'

let cancel = ref false
let rad = 0.07
let SPEED = 0.005
let BOX = Box(Plane.WorldXY, [Point3d(0.,0.,0.) ; Point3d(1.,1.,1.)] )


// Geometry.Constructors
let v3f (v:Vector3d) = Vector3f(float32 v.X, float32 v.Y, float32 v.Z)
let p3f (v:Point3d) = Point3f(float32 v.X, float32 v.Y, float32 v.Z)


// Geometry.Propositions
let within (limit):
    let prop(line, point) =
        limit >= line.DistanceTo point true
    prop


// Geometry.Measures
let distance (p1: Point3d, p2: Point3d) =
    (p1 - p2).Length

// Geometry.Transforms
let average (points) =
    let mutable cen = Point3d()
        for vertex in  ball.mesh.Vertices do
            cen <- cen + vertex
    cen <- cen / float ball.mesh.Vertices.Count 

let ranVec() =
    Vector3d(rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble()) 

// Core.Ball
type Ball (cen:Point3d, line:Geometry.Line) as ball =
    let cen = cen
    let sphere = Sphere point rad
    let mesh = Mesh.CreateFromSphere sphere 15 15
    let color = color
    let line = line

    method ball.distance (ball2: Ball) =
        distance(ball.cen, ball2.cen) - 2 * rad

    method ball.move (vector) =
        ball.cen <- ball.cen + vector
        moveMesh ball.mesh vector
        self

let face_vert_indices(face):
    let len = match 
    = [0..len] |> List.map(face.Item)
            let face_verts = face_vert_indices |> List.map(V.get)
            let face_center =  average face_verts

// Core.ball.Measures
let 

// Core.Balls
type Balls () =
    inherit Rarr()

    method self.collisions () =
        let collisions = Balls()

        for i = 0 to balls.Count - 1  do
            for j = i + 1 to balls.Count - 1 do
                if balls.[i].distance(balls.[j]) < 0 then
                    collisions.Add balls.[i]
                    collisions.Add balls.[j]
        collisions

// Core.BallBehaviors
// Travel
let updateBall (ball:Ball) =
    let velocity = ball.line.Direction * SPEED
    let cen' = ball.cen + velocity

    if not within(0.01) ball.line cen':
        let velocity = -velocity
        ball.line.Flip()

    ball.move velocity

let explodeBall (ball:Ball) =
    ball.mesh.Unweld(0.,true)


        let mesh = ball.mesh
        let vertices = mesh.Vertices
        let faces = mesh.Faces

        let mesh_center = average(vertices)


        for face in ball.mesh.Faces do
            let face_center = face.GetFaceCenter()
            let direction = (face_center - mesh_center) * 0.02
            
            update_face_verts(fun (p) ->
                p + direction + ran() * 0.005
            )

// Core.BallsBehaviors
let do_balls_behaviors (balls:Balls) =
    match balls.collisions with
        |[] -> for ball in balls do updateBall ball
        | collisions ->
            do collisions |> List.map(explodeBall)
        Error(balls)
        


// Core.ViewComputation
let getCamline(view:RhinoViewport)  = 
    let camline = Line(view.CameraLocation , view.CameraDirection )           
    camline.Extend(1000.,1000.) |> ignore
    let result, intersection = LineBox(camline, BOX ,0.01)

    if result then 
        let p1 = line.PointAt intersection.T0
        let p2 = line.PointAt intersection.T1
        let trimmedcamline = Line(p1, p2)

        if trimmedcamline.Length > 0.2 then
            Some trimmedcamline
    None


let getCrosshair (view:RhinoViewport)  = 

    let crosshair (point, vector) = 
        let vector' = unitize(vector) * rad
        Line(point - vector', point + vector') 

    match getCamline view with
    | Some camline -> 
        let cen = camline.From
        let x_axis, y_axis = view.CameraX, view.CameraY
        let x_crosshair = crosshair(cen, x_axis)
        let y_crosshair = crosshair(cen, y_axis)
        Some(x_crosshair, y_crosshair)
    | None -> None


let addBall (view:RhinoViewport) =
    match getCamline view with
    | Some camline ->
        let cen = camline.From
        Some Ball(cen, camline, Color.Red)
    | None -> None


// Build Rhino conduit
type Conduit () =
    inherit DisplayConduit()

    let balls = Balls()

    member this.AddBalls (view:RhinoViewport) =
        match addBall(view) with
        | Some ball -> balls.add
        | None -> None


    override this.PreDrawObjects (drawEventArgs:DrawEventArgs) =                

        let view = drawEventArtgs.Viewport
        let display = drawEventArtgs.Display

        base.PreDrawObjects(drawEventArgs)

        match getCrosshair view with
        |Some(x_crosshair, y_crosshair) ->
            display.DrawLine (x_crosshair, Color.Blue)
            display.DrawLine (y_crosshair, Color.Blue)
        |None -> ()

        balls <- do_balls_behaviors(balls)
            

        let mat = new DisplayMaterial (new Rhino.DocObjects.Material())
        for ball in balls do
            e.Display.DrawDottedLine (ball.line, ball.color) 
            e.Display.DrawMeshShaded (ball.mesh, mat)   

        let cor = Point2d(150.,150.)
        let tx = sprintf "Lines: %d" balls.Count
        //e.Display.Draw2dText(tx,Color.Red,cor,true)
        e.Display.Draw2dText(tx,Color.Gray,cor,true, 50)
        e.Display.EnableDepthWriting true
        e.Display.EnableDepthTesting true

        
