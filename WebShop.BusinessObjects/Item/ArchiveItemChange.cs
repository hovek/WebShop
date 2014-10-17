using Syrilium.DataAccessInterface;
using Syrilium.DataAccessInterface.SQL;
using Syrilium.Modules.BusinessObjects;
using Syrilium.Modules.BusinessObjects.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebShop.BusinessObjects.ModuleDefinitions;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;

namespace WebShop.BusinessObjects.Item
{
    public class ArchiveItemChange : IArchiveItemChange
    {
        public IItem PreviousItem { get; set; }
        public IItem CurrentItem { get; set; }
        public List<int> DeletedAttributes { get; set; }

        public int ItemId { get; set; }

        public ArchiveItemChange()
        {
            DeletedAttributes = new List<int>();
        }

        private bool? isItemInNewsletterSubscription = null;
        public bool IsItemInNewsletterSubscription
        {
            get
            {
                if (isItemInNewsletterSubscription == null)
                {
                    SySqlParameter param = new SySqlParameter();
                    param.ParameterName = "@ItemId";
                    param.Value = ItemId;
                    List<SySqlParameter> parameters = new List<SySqlParameter>();
                    parameters.Add(param);

                    param = new SySqlParameter();
                    param.ParameterName = "@LanguageId";
                    param.Value = Module.LanguageId;
                    parameters.Add(param);

                    isItemInNewsletterSubscription = Module.I<IQuery>().GetScalarValueWithProcedure<bool>(SPNames.IsItemInNewsletterSubscription, parameters);
                }
                return isItemInNewsletterSubscription.Value;
            }
        }

        public void SetPreviousItem(int itemId)
        {
            ItemId = itemId;
            if (IsItemInNewsletterSubscription)
                PreviousItem = Module.I<IItem>().Get(itemId);
        }

        private List<ArchiveAttributeChange> getDifferences(List<KeyValuePair<IItemAttribute, IItemAttribute>> attributePairs)
        {
            int languageId = Module.LanguageId;
            List<ArchiveAttributeChange> archiveAttributeChanges = new List<ArchiveAttributeChange>();

            foreach (var attributePair in attributePairs)
            {
                ArchiveAttributeChange archiveAttributeChange = new ArchiveAttributeChange();

                List<IItemValue<int>> intPrev = new List<IItemValue<int>>();
                List<IItemValue<string>> stringPrev = new List<IItemValue<string>>();
                List<IItemValue<decimal>> decimalPrev = new List<IItemValue<decimal>>();
                List<IItemValue<DateTime>> dateTimePrev = new List<IItemValue<DateTime>>();
                List<IItemValue<int>> intCurr = new List<IItemValue<int>>();
                List<IItemValue<string>> stringCurr = new List<IItemValue<string>>();
                List<IItemValue<decimal>> decimalCurr = new List<IItemValue<decimal>>();
                List<IItemValue<DateTime>> dateTimeCurr = new List<IItemValue<DateTime>>();

                if (attributePair.Key == null)
                {
                    archiveAttributeChange.Added = true;
                    archiveAttributeChange.AttributeId = attributePair.Value.AttributeId;
                }
                else
                {
                    archiveAttributeChange.AttributeId = attributePair.Key.AttributeId;
                    intPrev = attributePair.Key.IntValues.Where(i => i.LanguageId == languageId).ToList();
                    stringPrev = attributePair.Key.StringValues.Where(i => i.LanguageId == languageId).ToList();
                    decimalPrev = attributePair.Key.DecimalValues.Where(i => i.LanguageId == languageId).ToList();
                    dateTimePrev = attributePair.Key.DateTimeValues.Where(i => i.LanguageId == languageId).ToList();
                }
                if (attributePair.Value == null)
                {
                    archiveAttributeChange.Deleted = true;
                    archiveAttributeChange.AttributeId = attributePair.Key.AttributeId;
                }
                else
                {
                    intCurr = attributePair.Value.IntValues.Where(i => i.LanguageId == languageId).ToList();
                    stringCurr = attributePair.Value.StringValues.Where(i => i.LanguageId == languageId).ToList();
                    decimalCurr = attributePair.Value.DecimalValues.Where(i => i.LanguageId == languageId).ToList();
                    dateTimeCurr = attributePair.Value.DateTimeValues.Where(i => i.LanguageId == languageId).ToList();
                }

                bool differs = intPrev.Count != intCurr.Count
                                || stringPrev.Count != stringCurr.Count
                                || decimalPrev.Count != decimalCurr.Count
                                || dateTimePrev.Count != dateTimeCurr.Count;

                if (!differs)
                {
                    for (int i = 0; i < intPrev.Count; i++)
                    {
                        if (intPrev[i].Value != intCurr[i].Value)
                        {
                            differs = true;
                            break;
                        }
                    }
                    for (int i = 0; i < stringPrev.Count; i++)
                    {
                        if (stringPrev[i].Value != stringCurr[i].Value)
                        {
                            differs = true;
                            break;
                        }
                    }
                    for (int i = 0; i < decimalPrev.Count; i++)
                    {
                        if (decimalPrev[i].Value != decimalCurr[i].Value)
                        {
                            differs = true;
                            break;
                        }
                    }
                    for (int i = 0; i < dateTimePrev.Count; i++)
                    {
                        if (dateTimePrev[i].Value != dateTimeCurr[i].Value)
                        {
                            differs = true;
                            break;
                        }
                    }
                }

                if (differs || archiveAttributeChange.Added || archiveAttributeChange.Deleted)
                    archiveAttributeChanges.Add(archiveAttributeChange);
            }

            return archiveAttributeChanges;
        }

        public void CompareAndSave()
        {
            List<KeyValuePair<IItemAttribute, IItemAttribute>> attributePairs = new List<KeyValuePair<IItemAttribute, IItemAttribute>>();
            foreach (IItemAttribute prevAttribute in PreviousItem.Attributes)
            {
                IItemAttribute currAttribute = DeletedAttributes.Exists(a => a == prevAttribute.AttributeId) ? null :
                                                CurrentItem.Attributes.Find(a => a.AttributeId == prevAttribute.AttributeId);
                attributePairs.Add(new KeyValuePair<IItemAttribute, IItemAttribute>(prevAttribute, currAttribute));
            }
            foreach (IItemAttribute currAttribute in CurrentItem.Attributes)
            {
                IItemAttribute prevAttribute = PreviousItem.Attributes.Find(a => a.AttributeId == currAttribute.AttributeId);
                if (prevAttribute == null) attributePairs.Add(new KeyValuePair<IItemAttribute, IItemAttribute>(prevAttribute, currAttribute));
            }

            List<ArchiveAttributeChange> changes = getDifferences(attributePairs);

            if (changes.Count > 0)
            {
                ItemChangeHistory ich = new ItemChangeHistory();
                ich.ItemId = CurrentItem.Id;
                ich.LanguageId = Module.LanguageId;
                ich.XmlChangeData = generateXml(changes);

                WebShopDb context = WebShopDb.I;
                context.ItemChangeHistory.Add(ich);
                context.SaveChanges();
            }
        }

        private string generateXml(List<ArchiveAttributeChange> changes)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<history>");
            foreach (ArchiveAttributeChange change in changes)
            {
                string changeType;
                if (change.Added)
                    changeType = "added";
                else if (change.Deleted)
                    changeType = "deleted";
                else
                    changeType = "changed";

                sb.Append(string.Concat("<attribute id=\"", change.AttributeId, "\" changeType=\"", changeType, "\"/>"));
            }
            sb.Append("</history>");

            return sb.ToString();
        }
    }

    public class ArchiveAttributeChange
    {
        public int AttributeId { get; set; }
        public List<dynamic> Value { get; set; }
        public bool Added { get; set; }
        public bool Deleted { get; set; }
    }
}
