//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WindowsFormsApp1
{
    using System;
    using System.Collections.Generic;
    
    public partial class Bill
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Bill()
        {
            this.BillInfoes = new HashSet<BillInfo>();
        }
    
        public int ID { get; set; }
        public System.DateTime DateCheckIn { get; set; }
        public Nullable<System.DateTime> DateCheckOut { get; set; }
        public int IDTable { get; set; }
        public int Status { get; set; }
        public Nullable<int> Discount { get; set; }
        public Nullable<double> TotalPrice { get; set; }
    
        public virtual TableInfo TableInfo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BillInfo> BillInfoes { get; set; }
    }
}
