using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PalverZ.Recycler
{
    
    

    //TODO: Clean the code, make it tweakable via config: configure what kind of part to look for
    //TODO: and what kind of alternator, or other part module to add.
    /// <summary>
    /// This part module will find the part attached after it check if it is LFO engine
    /// and add a monoprop alternator to it.
    /// </summary>   

    public class ModuleExhaustRecycler :PartModule
    {
        private Part childPart;
        public override void OnStart(StartState state)
        {
            childPart = part.FindChildParts<Part>()[0];
            if (PartHasModule(childPart))
                childPart.AddModule("ModuleExhaustCapture");
            
            
             
        }

        bool PartHasModule(Part qPart, string name = " ") {
            print("PZER: Checking " + qPart.partName);
            foreach (PartModule module in qPart.Modules) {

                if (module is ModuleEngines) 
                {
                    ModuleEngines engine = module as ModuleEngines;
                    if (engine.engineType == EngineType.LiquidFuel)
                        print("PZER: Attached to LFO engine");
                        return true;
                }
            
            }
            print("PZER:  Not Attached to LFO engine");
            return false;
        }
    }
    
    //TODO make this configurable
    /// <summary>
    /// This is the alternator the other module will add
    /// </summary>
    public class ModuleExhaustCapture : ModuleAlternator 
    {
        
        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            moduleName = "Exhaust Capture";
            List<ModuleResource> resList = new List<ModuleResource>();
            ModuleResource outPutRes = new ModuleResource();
            outPutRes.name = "MonoPropellant";
            outPutRes.id = PartResourceLibrary.Instance.GetDefinition("MonoPropellant").id; //Do I need this
            outPutRes.rate = 1.5;
            resList.Add(outPutRes);
            outputResources = resList;
            

        }
    
    }
}
