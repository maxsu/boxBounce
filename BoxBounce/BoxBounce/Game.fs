namespace BoxBounce

open Rhino

open Rhino.Display.DisplayPipeline
open Rhino.Input
open Rhino.Geometry
open Rhino.Commands
open GosLib
open Microsoft.FSharp.Collections
open System.Diagnostics
open Rhino.RhinoApp


module rs = GosLib.RhinoScriptSyntax

SPACE = 32
ESC = 27
RET = 13


type keyFactory(hotkey, effect) =
    hook key = 
        if key = hotkey 
        then effect()
    
    hookEvent = KeyboardHookEvent hook
    
    RhinoApp.add_KeyboardEvent _event
    
    member this.Dispose =
        RhinApp.removeKeyboardEvent hookEvent

type Limiter time  = 
    let stopWatch = Stopwatch.StartNew()

    member self.limit effect =
            if stopWatch.EllapsedMilliseconds > time
            then
                effect()
                stopWatch.Restart()

module Game =
           
    let run (doc:Rhino.RhinoDoc)  =
        
        let conduit = Core.Conduit()
        use drawer = PreDrawObjects.Subscribe conduit.PDO
        let view = doc.Views.ActiveView.ActiveViewport()
        let rate = Limiter 300
        
        // Commands
        let addBall = rate.limit(
            fun () -> 
                conduit.AddBall(view)
                doc.Views.Redraw()
        )
        
        let quitGame = Core.cancel := true
        
        // Key mappings
        use keySpc = KeyFactory SPACE addBall
        use keyEsc = KeyFactory ESC quitGame


        // Main loop
        while not !Core.cancel do      
            doc.Views.Redraw()  
            Rhino.RhinoApp.Wait()

        // Cleanup
        Core.cancel := false
