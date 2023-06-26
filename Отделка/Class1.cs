using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WorkApp.Отделка
{
    
    public struct StyleByFamily
    {
        public string Name;
        public string ceil;
        public string wall;
        public string floor;
        public string kolon;
        public string plintus;
        public string other;
    }

    public struct CheckBoxes 
    {
        public bool room_names;
        public bool simple_names;
        public bool recon;
        public bool some_levels;
        public bool diff_levels;
        public bool grouped;
        public bool main_type;
        public bool local_type;
        public bool kolon_type;
        public bool finish_mode;
    }

    public struct ParamFields
    {
        public string group_finish;
        public string group_floor;
        public string prefix_finish;
        public string prefix_floor;
        public string sostav;
        public string sostav_ceil;
        public string wall_func;
        public string note_finish;
        public string note_floor;
        public string endnote_finish;
        public string endnote_floor;
        public string floor_mark;

    }
}
