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
    
    public partial class Account
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Account()
        {
            this.FamilyMembers = new HashSet<FamilyMember>();
            this.FamilyMembers1 = new HashSet<FamilyMember>();
            this.Lessons = new HashSet<Lesson>();
        }
    
        public int ID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int AccountType_ID { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string ZoomUserId { get; set; }
        public string Description { get; set; }
        public string Qualifications { get; set; }
        public Nullable<int> Rate { get; set; }
        public string Picture { get; set; }
        public Nullable<bool> Disabled { get; set; }
        public Nullable<bool> Hidden { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FamilyMember> FamilyMembers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FamilyMember> FamilyMembers1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lesson> Lessons { get; set; }
    }
}
