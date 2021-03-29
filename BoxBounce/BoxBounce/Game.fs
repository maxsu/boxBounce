namespace BoxBounce

open System
open System
open Rhino
open Rhino.Input
open Rhino.Geometry
open Rhino.Commands
open GosLib
open Microsoft.FSharp.Collections

module rs = GosLib.RhinoScriptSyntax


module Game =
    
       
    let run (doc:Rhino.RhinoDoc)  =
        
        let stopWatch = System.Diagnostics.Stopwatch.StartNew()
        let conduit = Core.Conduit()

        //Display.DisplayPipeline.PreDrawObjects.Add conduit.PDO
        let drawer = Display.DisplayPipeline.PreDrawObjects.Subscribe conduit.PDO

        let addBall i = 
            if i = 32  && stopWatch.ElapsedMilliseconds > 300L then // spaceaber is 32
                stopWatch.Restart() 
                conduit.AddBall(doc.Views.ActiveView.ActiveViewport)
                doc.Views.Redraw() 
        
        let keyEsc = RhinoApp.KeyboardHookEvent( fun i -> if i = 27 then Core.cancel := true ) // rs.print2 "Key:" i
        let keySpc = RhinoApp.KeyboardHookEvent addBall // esc=27, enter=13, space = 32  
 
        RhinoApp.add_KeyboardEvent keyEsc
        RhinoApp.add_KeyboardEvent keySpc

        while not !Core.cancel do      
            doc.Views.Redraw()  
            Rhino.RhinoApp.Wait()

        RhinoApp.remove_KeyboardEvent keyEsc
        RhinoApp.remove_KeyboardEvent keySpc

        drawer.Dispose()
        Core.cancel := false
        //Display.DisplayPipeline.CalculateBoundingBox.RemoveHandler conduit.CBB
        //Display.DisplayPipeline.PreDrawObjects.RemoveHandler conduit.PDO
