//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CcsEnityFramework
{
    using System;
    using System.Collections.Generic;
    
    public partial class onbase_doc_type
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public onbase_doc_type()
        {
            this.doc_type_mapping = new HashSet<doc_type_mapping>();
        }
    
        public short id { get; set; }
        public string name { get; set; }
        public Nullable<long> ob_itemtypenum_prod { get; set; }
        public Nullable<long> ob_itemtypenum_test { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<doc_type_mapping> doc_type_mapping { get; set; }
    }
}
