using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.BusinessObjectsInterface.Item;
using S = Syrilium.Modules.BusinessObjects;

namespace WebShop.Models
{
    public class GroupEdit
    {
        public int? SelectedGroupId { get; set; }
        public int? GroupId { get; set; }
        public string Action { get; set; }

        private IItem group;
        public IItem Group
        {
            get
            {
                if (group == null && GroupId.HasValue)
                {
                    group = Module.I<IItem>().Get(GroupId.Value);
                }
                return group;
            }
        }

        public string Name
        {
            get
            {
                if (Action == "edit" && Group != null) return Group.GetValueFormated(AttributeKeyEnum.Name);
                return "";
            }
        }

        public bool Show
        {
            get
            {
                if (Action == "edit" && Group != null)
                {
                    bool? show = Group.GetRawValue(AttributeKeyEnum.Show);
                    if (!show.HasValue) return true;
                    return show.Value;
                }
                return true;
            }
        }
    }
}