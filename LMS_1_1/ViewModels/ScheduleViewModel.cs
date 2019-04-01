using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS_1_1.ViewModels
{
    public class ScheduleViewModel
    {
        public string Id { get; set; }
        public string Color { get; set; }

        public int? Length { get; set; }

        public int? Offsettime { get; set; }
        public string  Name { get; set; }

        public string Description { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public DayOfWeek? DayOfWeek { get; set; }

        public int? ActivityTypeId { get; set; }

        public int? Zindex { get; set; }
        public int? Width { get; set; }
        public int? Left { get; set; }
    }

    /*
     *     weekday: string="";
    id:string;
    color: string;
    length:number;
    Offsettime: number;
    name:string;
    description:string;
    */
}
