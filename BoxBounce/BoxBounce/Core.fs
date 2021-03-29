module Core
namespace BoxBounce
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


// Propositions
let within (limit):
    let prop(line, point) =
        limit >= line.DistanceTo point true
    prop

// Measures
let distance (p1: Point3d, p2: Point3d) =
    (p1 - p2).Length





type Ball (cen:Point3d, line:Geometry.Line) =
    let cen = cen
    let sphere = Sphere point rad
    let mesh = Mesh.CreateFromSphere sphere 15 15
    let color = color
    let line = line

    method self.move (vector) =
        self.cen = self.cen + vector
        moveMesh ball.mesh vector
        self

type Balls () =
    inherit Rarr()

    let self.collisions () =
        for i = 0 to balls.Count - 1 do
            let cen_1 = balls.[i].cen
            for j = i + 1 to balls.Count - 1 do
                let cen_2 = balls.[j].cen
                if distance(cen_1, cen_2) < 2 * rad then
                    explode.Add balls.[i] 
                    explode.Add balls.[j]


let v3f (v:Vector3d) = Vector3f(float32 v.X, float32 v.Y, float32 v.Z)


let p3f (v:Point3d) = Point3f(float32 v.X, float32 v.Y, float32 v.Z)


let updateBall (ball:Ball) =
    let velocity = ball.line.Direction * SPEED
    let cen' = ball.cen + velocity

    if not within(0.01) ball.line cen':
        let velocity = -velocity
        ball.line.Flip()

    ball.move velocity


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
    match getCamline view with      
    | Some camline -> 
        let x = unitize(view.CameraX) * rad
        let y = unitize(view.CameraY) * rad
        let cen = camline.From
        let x_crosshair = Line(cen-x,cen+x)
        let y_crosshair = Line(cen-y,cen+y)

        Some(x_crosshair, y_crosshair)

    | None -> None


// Build Rhino conduit
type Conduit () =
    inherit DisplayConduit()

    let balls = Balls()


    member this.AddBall  (view:RhinoViewport) =
        match getCamline view with
        | Some camline -> 
            let cen = camline.From
            let ball = Ball(cen, camline, Color.Red)

            balls.Add ball

        | None -> ()


    override this.PreDrawObjects (drawEventArgs:DrawEventArgs) =                

        let view = drawEventArtgs.Viewport
        let display = drawEventArtgs.Display
        let explode = Rarr()

        base.PreDrawObjects(drawEventArgs)

        match getCrosshair view with
        |Some(x_crosshair, y_crosshair) ->

            display.DrawLine (x_crosshair, Color.Blue)
            display.DrawLine (y_crosshair, Color.Blue)

        |None -> ()




        if explode.Count > 0 then
            for ball in explode do  
                ball.mesh.Unweld(0.,true)

                let mutable cen = Point3d()
                for p in  ball.mesh.Vertices do
                    cen <- cen + (Point3d( p))
                cen <- cen / float  ball.mesh.Vertices.Count 

                for face in ball.mesh.Faces do
                    let a = Point3d(ball.mesh.Vertices.[face.A])
                    let b = Point3d(ball.mesh.Vertices.[face.B])
                    let c = Point3d(ball.mesh.Vertices.[face.C])
                    let d = Point3d(ball.mesh.Vertices.[face.D])
                    let fc = (a+b+c+d) / 4.0

                    let dir = (fc - cen) * 0.02
                    let ran() = (Vector3d(rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble()))*0.005                        
                    ball.mesh.Vertices.[face.A] <- p3f (a + dir + ran() )
                    ball.mesh.Vertices.[face.B] <- p3f (b + dir + ran() )
                    ball.mesh.Vertices.[face.C] <- p3f (c + dir + ran() )
                    ball.mesh.Vertices.[face.D] <- p3f (d + dir + ran() )

                    if abs(a.X) > 4.0 then 
                        balls.Clear()
                        cancel := true

            let cor = Point2d(550.,290.)
            let tx = "Game Over"
            //e.Display.Draw2dText(tx,Color.Red,cor,true)
            e.Display.Draw2dText(tx,Color.Red,cor,true, 90)

        else
            for ball in balls do
                updateBall ball

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

    member this.PDO (e:DrawEventArgs) = this.PreDrawObjects e

        
