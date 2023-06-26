using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIFramework.PropertyGrid;

namespace WorkApp.now
{
    internal class Prototype
    {

    }

    class Wrapeer
    {
        Object element;
        /*
         * 
         * LIST OF GETTERS AND SETTERS
         * 
         */
    }
    class DecoPair
    {
        string Name;
        string Text;
        double value;
    }
    class Surface
    {
        Object surface;
        void Get() { }
    }
    class RoomWrap
    {
        List<DecoPair> pairs;
        List<Surface> surfaces;
    }
    class Processor
    {
        List<RoomWrap> rooms;
        public void Get() { }
        public void Make() { }
        public void Commit() { }
    }



    class Command
    {
        public void Execute()
        {
            var root = new Processor();
            root.Get();
            root.Make();
            root.Commit();

        }
    }

}
