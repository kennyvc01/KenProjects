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
    
    public partial class file
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public file()
        {
            this.onbase_file_info = new HashSet<onbase_file_info>();
        }
    
        public long id { get; set; }
        public Nullable<long> doc_id { get; set; }
        public long volume_id { get; set; }
        public string relative_path { get; set; }
        public int file_size { get; set; }
        public Nullable<int> file_type_id { get; set; }
        public Nullable<int> sb_file_type { get; set; }
        public Nullable<short> snapshot_id { get; set; }
        public int seq { get; set; }
        public Nullable<int> page_count { get; set; }
        public Nullable<int> x_dpi { get; set; }
        public Nullable<int> y_dpi { get; set; }
    
        public virtual document document { get; set; }
        public virtual file_type file_type { get; set; }
        public virtual snapshot snapshot { get; set; }
        public virtual legacy_file_info legacy_file_info { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<onbase_file_info> onbase_file_info { get; set; }
    }
}
