using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PalverZ.Recycler
{
    
    

    //TODO: Clean the code, make it more tweakable via config: configure what kind of part to look for
    //TODO: and what kind of alternator, or other part module to add.
    /// <summary>
    /// This part module will find the part attached after it check if it is LFO engine
    /// and add a monoprop alternator to it.
    /// </summary>   

    public class ModuleExhaustRecycler :PartModule
    {
        
        private double thrustMultiplier = 1;
        //TODO Make this able to handel a list for multiple by products?!?!?!?
        [KSPField]
        public string engineType = "LiquidFuel";
        [KSPField]
        public string primaryResourceName = "MonoPropellant";
        [KSPField]
        public double primaryResourceRate = 1;
        [KSPField]
        public string secondaryResourceName;
        [KSPField]
        public double secondaryResourceRate = 0;
        [KSPField]
        public double additionalHeatRate = .15;





        public override void OnStart(StartState state)
        {
            List<Part> children = part.FindChildParts<Part>().ToList<Part>();
            
            if (children == null || children.Count == 0)
                return; //Simple little way to avoid problems I hope 

            foreach (Part childPart in children) 
            {
                print("PZER: Child " + childPart.name);
                if (PartHasModule(childPart, engineType))
                    AttachCollector(childPart);
            }

            //childPart = part.FindChildParts<Part>()[0]; //Finds first child later Ill look at all direct children (for radial engine support)
            
                      
           
             
        }

        void AttachCollector(Part toPart) {
            toPart.AddModule("ModuleExhaustCapture");

            ModuleExhaustCapture newModule = toPart.FindModuleImplementing<ModuleExhaustCapture>();
            ModuleResource outPutRes = new ModuleResource();
            
            if (primaryResourceName != null && primaryResourceRate != 0 && PartResourceLibrary.Instance.resourceDefinitions.Contains(primaryResourceName)) 
            {
           
                outPutRes.name = primaryResourceName;
                outPutRes.id = PartResourceLibrary.Instance.GetDefinition(primaryResourceName).id; //Do I need this       
                outPutRes.rate = primaryResourceRate*thrustMultiplier;
           
                newModule.outputResources.Add(outPutRes);
                print("PZER: Capture " + outPutRes.name + " " + outPutRes.id + " added with a real rate of " + outPutRes.rate * thrustMultiplier);
            }
            else
            {
                 print("PZER: " + part.partName + " has bad primary resource/rate: " + primaryResourceName +" rate: " + primaryResourceRate);
            }

            
            
            if (secondaryResourceName != null && secondaryResourceRate != 0 && PartResourceLibrary.Instance.resourceDefinitions.Contains(secondaryResourceName)) {


                outPutRes.name = secondaryResourceName;
                 
                outPutRes.id = PartResourceLibrary.Instance.GetDefinition(secondaryResourceName).id; //see above       
                outPutRes.rate = secondaryResourceRate * thrustMultiplier;

  
                newModule.outputResources.Add(outPutRes);

                print("PZER: Secondary capture " + outPutRes.name + " " + outPutRes.id + " added with a real rate of " + outPutRes.rate * thrustMultiplier);
            }
            else
            
            {
                print("PZER: " + part.partName + " has bad secondary resource/rate: " + secondaryResourceName + " rate: " + secondaryResourceRate);
            }


            

            newModule.heatIncrease = (float)additionalHeatRate;



        }

        
        bool PartHasModule(Part qPart, string engineTypeName = "LiquidFuel") {
            print("PZER: Checking " + qPart.name);
            foreach (PartModule module in qPart.Modules) {
               
                if (module is ModuleEngines) 
                {
                    ModuleEngines engine = module as ModuleEngines;

                    if (engine.engineType == (EngineType)Enum.Parse(typeof(EngineType), engineTypeName, true)) //TODO Catch bad types?
                    {
                        print("PZER: Attached to " + engineTypeName + " engine type");
                        thrustMultiplier = engine.maxThrust / 215; //use the maxrate of the LV-T30
                        return true;
                    }
                }
            
            }
            print("PZER:  Not Attached to " + engineTypeName + " engine type");
            return false;
        }
    }
    
    
    /// <summary>
    /// This is the alternator the other module will add
    /// This will probably get deprecated one day
    /// </summary>
    public class ModuleExhaustCapture : ModuleAlternator 
    {
        public float heatIncrease;
        ModuleEngines myEngine;
        
        
        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            moduleName = "Exhaust Capture";
            myEngine = part.FindModuleImplementing<ModuleEngines>();
            myEngine.heatProduction *= (1+heatIncrease);
            
    

        }
    
    }
}
