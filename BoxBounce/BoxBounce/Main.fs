namespace BoxBounce
open System.Runtime.InteropServices
open type Rhino.Commands.Result.Success
open Rhino.PlugIns.Plugin
open Rhino.Commands.Command

type private NewPlugIn() =
    inherit PlugIn()
    
    static public member val Instance = NewPlugIn()


[<Guid("ce66e7f8-7f16-476d-a30e-6540a04c25eb")>]
type BoxBounce() =
    inherit Command()
    
    static member val Instance = BoxBounce()
    
    override this.EnglishName = "BoxBounce"
           
    override this.RunCommand (doc, mode)  =        
        Game.run doc
     
        Success
        
