namespace BoxBounce
open GosLib

Success = Rhino.Commands.Result.Success

// Singleton
type NewPlugIn () =     
    inherit Rhino.PlugIns.PlugIn()
    static member val Instance = NewPlugIn()


// Singleton
[<System.Runtime.InteropServices.Guid("ce66e7f8-7f16-476d-a30e-6540a04c25eb")>]
type BoxBounce () =
    inherit Rhino.Commands.Command()    
    static member val Instance = BoxBounce()
    
    override this.EnglishName = "BoxBounce"
           
    override this.RunCommand (doc, mode)  =        
        Game.run doc
     
        Success
        
