using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using LS_ERP.Ultilities.Global;
using LS_ERP.Ultilities.Helpers;

namespace LS_ERP.XAF.Module.BusinessObjects {
    [DefaultProperty(nameof(UserName))]
    public class ApplicationUser : PermissionPolicyUser, IObjectSpaceLink, ISecurityUserWithLoginInfo {
        public ApplicationUser() : base() {
            UserLogins = new List<ApplicationUserLoginInfo>();
        }

        [Browsable(false)]
        [DevExpress.ExpressApp.DC.Aggregated]
        public virtual IList<ApplicationUserLoginInfo> UserLogins { get; set; }
        IEnumerable<ISecurityUserLoginInfo> IOAuthSecurityUser.UserLogins => UserLogins.OfType<ISecurityUserLoginInfo>();

        ISecurityUserLoginInfo ISecurityUserWithLoginInfo.CreateUserLoginInfo(string loginProviderName, string providerUserKey) {
            ApplicationUserLoginInfo result = ((IObjectSpaceLink)this).ObjectSpace.CreateObject<ApplicationUserLoginInfo>();
            result.LoginProviderName = loginProviderName;
            result.ProviderUserKey = providerUserKey;
            result.User = this;
            return result;
        }

        /// <summary>
        /// Additional information
        /// </summary>
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string NormalizedPhone { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public string Supervisor { get; set; }
        public string Signature { get; set; }

        [NotMapped]
        [ImageEditor(ListViewImageEditorMode = ImageEditorMode.PictureEdit,
           DetailViewImageEditorMode = ImageEditorMode.PictureEdit,
           DetailViewImageEditorFixedHeight = 150)]
        public byte[] ImageData
        {
            get => SaveFileHelpers.Dowload(this.Signature);
            set
            {
                if (value != null)
                {
                    this.Signature = SaveFileHelpers.Upload(value,
                        Nanoid.Nanoid.Generate("ABCDEFGHIJKLMONPRSTUV0123456789", 8) + ".png",
                        AppGlobal.UploadFileUrl);
                }
            }
        }
    }
}
