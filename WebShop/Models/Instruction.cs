using Syrilium.CommonInterface;
using Syrilium.CommonInterface.Caching;
using Syrilium.Modules.BusinessObjects;
using Syrilium.Modules.BusinessObjects.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.BusinessObjectsInterface;
using WebShop.BusinessObjectsInterface.Item;
using S = Syrilium.Interfaces.BusinessObjectsInterface.Item;

namespace WebShop.Models
{
    public class Instruction
    {
        public string Command { get; set; }
        public string Result { get; set; }

        public void Execute()
        {
            switch (Command)
            {
                //dodavanje atributa za ocjenjivanje
                case "MIG83gn(Gg28w3gn(ENG#379g":
                    List<IAttributeDefinition> attributes = Module.I<IAttributeDefinition>().Get();
                    WebShopDb context = WebShopDb.I;

                    if (!attributes.Exists(a => a.Key == AttributeKeyEnum.GradeCount))
                    {
                        context.ItemDefinition.Add(new ItemDefinition
                       {
                           TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                           IntData = new ObservableCollection<S.IItemData<int>> { 
					        new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Int } ,
                            new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingHidden, Value = 1 },
                            new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDeleteNotAllowed, Value = 1 },
					        new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDataTypeChangeNotAllowed, Value = 1 } ,
					        new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeKey, Value = (int)AttributeKeyEnum.GradeCount },
				        },
                           StringData = new ObservableCollection<S.IItemData<string>> { 
					        new ItemDefinitionDataString { TypeId = 1, Value = "Broj ocjena" },
					        new ItemDefinitionDataString { TypeId = 2, Value = "Grade count" }
				        },
                           Children = new ObservableCollection<S.IItem>{
					        new ItemDefinition { 
						        TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						        StringData = new ObservableCollection<S.IItemData<string>> { 
							        new ItemDefinitionDataString { TypeId = 1, Value = "Broj ocjena" },
							        new ItemDefinitionDataString { TypeId = 2, Value = "Grade count" }
						        }
					        }
				        }
                       });
                    }

                    if (!attributes.Exists(a => a.Key == AttributeKeyEnum.GradeSum))
                    {
                        context.ItemDefinition.Add(new ItemDefinition
                             {
                                 TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                                 IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Int } ,
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingHidden, Value = 1 },
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDeleteNotAllowed, Value = 1 },
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDataTypeChangeNotAllowed, Value = 1 } ,
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeKey, Value = (int)AttributeKeyEnum.GradeSum },
				},
                                 StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Zbroj ocjena" },
					new ItemDefinitionDataString { TypeId = 2, Value = "Grade sum" }
				},
                                 Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Zbroj ocjena" },
							new ItemDefinitionDataString { TypeId = 2, Value = "Grade sum" }
						}
					}
				}
                             });
                    }

                    if (!attributes.Exists(a => a.Key == AttributeKeyEnum.Grade))
                    {
                        context.ItemDefinition.Add(new ItemDefinition
                                {
                                    TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                                    IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Decimal } ,
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingHidden, Value = 1 },
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDeleteNotAllowed, Value = 1 },
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDataTypeChangeNotAllowed, Value = 1 } ,
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeKey, Value = (int)AttributeKeyEnum.Grade },
				},
                                    StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Ocjena" },
					new ItemDefinitionDataString { TypeId = 2, Value = "Grade" }
				},
                                    Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Ocjena" },
							new ItemDefinitionDataString { TypeId = 2, Value = "Grade" }
						}
					}
				}
                                });
                    }

                    if (context.SaveChanges() > 0)
                    {
                        Result = "Atributi za ocjenjivanje su dodani";
                        Module.I<ICache>(CacheNames.MainCache).ClearAll();
                        Module.I<ICache>(CacheNames.AdminCache).ClearAll();
                    }
                    else Result = "Atributi već postoje.";
                    break;
                default:
                    Result = "Nepostojeća naredba.";
                    break;
            }
        }
    }
}