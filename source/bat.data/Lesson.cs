//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace bat.data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Lesson
    {
        public int ID { get; set; }
        public int Account_ID { get; set; }
        public System.DateTime BookingDate { get; set; }
        public string Description { get; set; }
        public int ClassSize { get; set; }
    
        public virtual Account Account { get; set; }
    }
}
