using DevExpress.Persistent.Base;
using LS_ERP.Ultilities.Global;
using LS_ERP.Ultilities.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class PackingListImageThumbnail : Audit
    {
        public long ID { get; set; }
        public int PackingListID { get; set; }
        public string ImageUrl { get; set; }
        public int? SortIndex { get; set; }

        [NotMapped]
        [ImageEditor(ListViewImageEditorMode = ImageEditorMode.PictureEdit,
           DetailViewImageEditorMode = ImageEditorMode.PictureEdit,
            ListViewImageEditorCustomHeight = 80,
           DetailViewImageEditorFixedHeight = 150)]
        [VisibleInListView(true)]
        public byte[] ImageData
        {
            get => SaveFileHelpers.Dowload(this.ImageUrl);
            set
            {
                if (value != null)
                {
                    this.ImageUrl = SaveFileHelpers.Upload(value,
                        Nanoid.Nanoid.Generate("ABCDEFGHIJKLMONPRSTUV0123456789", 8) + ".png",
                        AppGlobal.UploadFileUrl);
                }
            }
        }
        public virtual PackingList PackingList { get; set; }
    }
}
