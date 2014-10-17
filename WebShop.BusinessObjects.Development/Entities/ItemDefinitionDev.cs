using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syrilium.Modules.BusinessObjects;
using Syrilium.Modules.BusinessObjects.Item;
using S = Syrilium.Interfaces.BusinessObjectsInterface.Item;
using WebShop.BusinessObjectsInterface.Item;
using Syrilium.CommonInterface;

namespace WebShop.BusinessObjects.Development.Entities
{
    public class ItemDefinitionDev
    {
        public static void ExecuteDDL(WebShopDb context)
        {
            #region ItemDefinition
            context.Database.ExecuteSqlCommand(
                @"SET ANSI_NULLS ON

				ALTER TABLE ItemDefinition
				ADD Node hierarchyid not null,
					ObjectNode hierarchyid not null

				CREATE UNIQUE NONCLUSTERED INDEX [IX_Node] ON [dbo].[ItemDefinition] 
				(
					[Node] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

				CREATE NONCLUSTERED INDEX [IX_ObjectNode] ON [dbo].[ItemDefinition] 
				(
					[ObjectNode] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
				
				CREATE NONCLUSTERED INDEX [IX_TypeId] ON [dbo].[ItemDefinition] 
				(
					[TypeId] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]");

            string objectIds = string.Format("{0}, {1}, {2}", (int)ItemDefinitionTypeEnum.AttributeDefinition, (int)ItemDefinitionTypeEnum.PredefinedList, (int)ItemDefinitionTypeEnum.AttributeTemplate);

            context.Database.ExecuteSqlCommand(
                @"CREATE TRIGGER [dbo].[ItemDefinitionInsteadOfInsert] ON [dbo].[ItemDefinition]
					INSTEAD OF INSERT
				AS
				BEGIN
					SET ANSI_WARNINGS OFF
					SET NOCOUNT ON 

					IF ( SELECT	COUNT(*)
						 FROM	INSERTED
					   ) > 1 
						BEGIN 
							RAISERROR ('Only single row at a time can be inserted.', -- Message text.
																				18, -- Severity,
																				1, -- State,
																				N'')
							RETURN 
						END 

					DECLARE	@MinNode HIERARCHYID           
					DECLARE	@MaxNode HIERARCHYID     
           
					IF ( SELECT	MAX(ParentId)
						 FROM	INSERTED
					   ) IS NOT NULL 
						BEGIN         
							SELECT	@MinNode = MIN(i.Node),
									@MaxNode = MAX(i.Node)
							FROM	INSERTED ins
							LEFT JOIN dbo.ItemDefinition i ON ins.ParentId = i.ParentId
						END
					ELSE 
						BEGIN
							SELECT	@MinNode = MIN(i.Node),
									@MaxNode = MAX(i.Node)
							FROM	dbo.ItemDefinition i
							WHERE	i.ParentId IS NULL 
						END 

					DECLARE	@LeftChild HIERARCHYID
					DECLARE	@RightChild HIERARCHYID
					IF @MinNode IS NOT NULL 
						BEGIN
							DECLARE	@FirstRelative HIERARCHYID       
							SET @FirstRelative = HIERARCHYID::Parse(@MinNode.GetAncestor(1).ToString() + '1/')
							IF @FirstRelative < @MinNode 
								BEGIN
									SET @RightChild = @MinNode
								END           
							ELSE 
								BEGIN
									SET @LeftChild = @MaxNode
								END           
						END
		  
					INSERT	INTO dbo.ItemDefinition
							( TypeId,
							  ParentId,
							  Node,
							  ObjectNode )
							SELECT	i.TypeId,
									i.ParentId,
									i.Node,
									ISNULL(o.Node, i.Node)
							FROM	( SELECT	i.TypeId,
												i.ParentId,
												CASE WHEN p.Id IS NULL THEN HIERARCHYID::GetRoot().GetDescendant(@LeftChild, @RightChild)
													 ELSE p.Node.GetDescendant(@LeftChild, @RightChild)
												END AS Node
									  FROM		INSERTED i
									  LEFT JOIN ItemDefinition p ON p.Id = i.ParentId
									) i
							OUTER APPLY ( SELECT TOP 1
													o.Node
										  FROM		dbo.ItemDefinition o
										  WHERE		isnull(i.TypeId,0) NOT IN ( " + objectIds + @" ) AND o.TypeId IN ( " + objectIds + @" ) AND i.Node.IsDescendantOf(o.Node) = 1
										  ORDER BY	o.Node DESC
										) o

					SELECT	SCOPE_IDENTITY() AS Id
				END");
            #endregion
            #region ItemDefinitionDataString
            context.Database.ExecuteSqlCommand(
                @"ALTER TABLE ItemDefinitionDataString
				ADD Node hierarchyid not null,
					ObjectNode hierarchyid not null

				CREATE NONCLUSTERED INDEX [IX_Node] ON [dbo].[ItemDefinitionDataString] 
				(
					[Node] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

				CREATE NONCLUSTERED INDEX [IX_ObjectNode] ON [dbo].[ItemDefinitionDataString] 
				(
					[ObjectNode] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

				CREATE NONCLUSTERED INDEX [IX_TypeId] ON [dbo].[ItemDefinitionDataString] 
				(
					[TypeId] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

				CREATE NONCLUSTERED INDEX [IX_Value] ON [dbo].[ItemDefinitionDataString] 
				(
					[Value] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]");

            context.Database.ExecuteSqlCommand(
                @"CREATE TRIGGER ItemDefinitionDataStringInsteadOfInsert ON dbo.ItemDefinitionDataString
					INSTEAD OF INSERT
				AS
				BEGIN
					INSERT	INTO dbo.ItemDefinitionDataString
							( TypeId,
							  ItemDefinitionId,
							  Value,
								Node,
							  ObjectNode )
							SELECT	i.TypeId,
									i.ItemDefinitionId,
									i.Value,
									it.Node,
									it.ObjectNode
							FROM	INSERTED i
							LEFT JOIN dbo.ItemDefinition it ON i.ItemDefinitionId = it.Id

					SELECT	SCOPE_IDENTITY() AS Id
				END");

            context.Database.ExecuteSqlCommand(
                @"CREATE TRIGGER ItemDefinitionDataStringAfterUpdate ON [dbo].[ItemDefinitionDataString]
					AFTER UPDATE
				AS
				UPDATE	id
				SET		Node = i.Node,
						ObjectNode = i.ObjectNode
				FROM	INSERTED ins
				INNER JOIN DELETED del ON ins.Id = del.Id
										  AND ins.ItemDefinitionId != del.ItemDefinitionId
				INNER JOIN ItemDefinitionDataString id ON ins.Id = id.Id
				INNER JOIN dbo.ItemDefinition i ON id.ItemDefinitionId = i.Id");
            #endregion
            #region ItemDefinitionDataInt
            context.Database.ExecuteSqlCommand(
                @"ALTER TABLE ItemDefinitionDataInt
				ADD Node hierarchyid not null,
					ObjectNode hierarchyid not null

				CREATE NONCLUSTERED INDEX [IX_Node] ON [dbo].[ItemDefinitionDataInt] 
				(
					[Node] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

				CREATE NONCLUSTERED INDEX [IX_ObjectNode] ON [dbo].[ItemDefinitionDataInt] 
				(
					[ObjectNode] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
				
				CREATE NONCLUSTERED INDEX [IX_TypeId] ON [dbo].[ItemDefinitionDataInt] 
				(
					[TypeId] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

				CREATE NONCLUSTERED INDEX [IX_Value] ON [dbo].[ItemDefinitionDataInt] 
				(
					[Value] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]");

            context.Database.ExecuteSqlCommand(
                @"CREATE TRIGGER ItemDefinitionDataIntInsteadOfInsert ON dbo.ItemDefinitionDataInt
					INSTEAD OF INSERT
				AS
				BEGIN
					INSERT	INTO dbo.ItemDefinitionDataInt
							( TypeId,
							  ItemDefinitionId,
							  Value,
								Node,
							  ObjectNode )
							SELECT	i.TypeId,
									i.ItemDefinitionId,
									i.Value,
									it.Node,
									it.ObjectNode
							FROM	INSERTED i
							LEFT JOIN dbo.ItemDefinition it ON i.ItemDefinitionId = it.Id

					SELECT	SCOPE_IDENTITY() AS Id
				END");

            context.Database.ExecuteSqlCommand(
                @"CREATE TRIGGER ItemDefinitionDataIntAfterUpdate ON [dbo].[ItemDefinitionDataInt]
					AFTER UPDATE
				AS
				UPDATE	id
				SET		Node = i.Node,
						ObjectNode = i.ObjectNode
				FROM	INSERTED ins
				INNER JOIN DELETED del ON ins.Id = del.Id
										  AND ins.ItemDefinitionId != del.ItemDefinitionId
				INNER JOIN ItemDefinitionDataInt id ON ins.Id = id.Id
				INNER JOIN dbo.ItemDefinition i ON id.ItemDefinitionId = i.Id");
            #endregion
            #region ItemDefinitionDataDecimal
            context.Database.ExecuteSqlCommand(
                @"ALTER TABLE ItemDefinitionDataDecimal
				ADD Node hierarchyid not null,
					ObjectNode hierarchyid not null

				CREATE NONCLUSTERED INDEX [IX_Node] ON [dbo].[ItemDefinitionDataDecimal] 
				(
					[Node] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

				CREATE NONCLUSTERED INDEX [IX_ObjectNode] ON [dbo].[ItemDefinitionDataDecimal] 
				(
					[ObjectNode] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

				CREATE NONCLUSTERED INDEX [IX_TypeId] ON [dbo].[ItemDefinitionDataDecimal] 
				(
					[TypeId] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

				CREATE NONCLUSTERED INDEX [IX_Value] ON [dbo].[ItemDefinitionDataDecimal] 
				(
					[Value] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]");

            context.Database.ExecuteSqlCommand(
                @"CREATE TRIGGER ItemDefinitionDataDecimalInsteadOfInsert ON dbo.ItemDefinitionDataDecimal
					INSTEAD OF INSERT
				AS
				BEGIN
					INSERT	INTO dbo.ItemDefinitionDataDecimal
							( TypeId,
							  ItemDefinitionId,
							  Value,
								Node,
							  ObjectNode )
							SELECT	i.TypeId,
									i.ItemDefinitionId,
									i.Value,
									it.Node,
									it.ObjectNode
							FROM	INSERTED i
							LEFT JOIN dbo.ItemDefinition it ON i.ItemDefinitionId = it.Id

					SELECT	SCOPE_IDENTITY() AS Id
				END");

            context.Database.ExecuteSqlCommand(
                @"CREATE TRIGGER ItemDefinitionDataDecimalAfterUpdate ON [dbo].[ItemDefinitionDataDecimal]
					AFTER UPDATE
				AS
				UPDATE	id
				SET		Node = i.Node,
						ObjectNode = i.ObjectNode
				FROM	INSERTED ins
				INNER JOIN DELETED del ON ins.Id = del.Id
										  AND ins.ItemDefinitionId != del.ItemDefinitionId
				INNER JOIN ItemDefinitionDataDecimal id ON ins.Id = id.Id
				INNER JOIN dbo.ItemDefinition i ON id.ItemDefinitionId = i.Id");
            #endregion
            #region ItemDefinitionDataDateTime
            context.Database.ExecuteSqlCommand(
                @"ALTER TABLE ItemDefinitionDataDateTime
				ADD Node hierarchyid not null,
					ObjectNode hierarchyid not null

				CREATE NONCLUSTERED INDEX [IX_Node] ON [dbo].[ItemDefinitionDataDateTime] 
				(
					[Node] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

				CREATE NONCLUSTERED INDEX [IX_ObjectNode] ON [dbo].[ItemDefinitionDataDateTime] 
				(
					[ObjectNode] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

				CREATE NONCLUSTERED INDEX [IX_TypeId] ON [dbo].[ItemDefinitionDataDateTime] 
				(
					[TypeId] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

				CREATE NONCLUSTERED INDEX [IX_Value] ON [dbo].[ItemDefinitionDataDateTime] 
				(
					[Value] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]");

            context.Database.ExecuteSqlCommand(
                @"CREATE TRIGGER ItemDefinitionDataDateTimeInsteadOfInsert ON dbo.ItemDefinitionDataDateTime
					INSTEAD OF INSERT
				AS
				BEGIN
					INSERT	INTO dbo.ItemDefinitionDataDateTime
							( TypeId,
							  ItemDefinitionId,
							  Value,
								Node,
							  ObjectNode )
							SELECT	i.TypeId,
									i.ItemDefinitionId,
									i.Value,
									it.Node,
									it.ObjectNode
							FROM	INSERTED i
							LEFT JOIN dbo.ItemDefinition it ON i.ItemDefinitionId = it.Id

					SELECT	SCOPE_IDENTITY() AS Id
				END");

            context.Database.ExecuteSqlCommand(
                @"CREATE TRIGGER ItemDefinitionDataDateTimeAfterUpdate ON [dbo].[ItemDefinitionDataDateTime]
					AFTER UPDATE
				AS
				UPDATE	id
				SET		Node = i.Node,
						ObjectNode = i.ObjectNode
				FROM	INSERTED ins
				INNER JOIN DELETED del ON ins.Id = del.Id
										  AND ins.ItemDefinitionId != del.ItemDefinitionId
				INNER JOIN ItemDefinitionDataDateTime id ON ins.Id = id.Id
				INNER JOIN dbo.ItemDefinition i ON id.ItemDefinitionId = i.Id");
            #endregion
            #region ItemDefinitionExtent
            context.Database.ExecuteSqlCommand(
                @"CREATE TRIGGER [dbo].[ItemDefinitionInsteadOfUpdate] ON [dbo].[ItemDefinition]
					INSTEAD OF UPDATE
				AS
				BEGIN
					DECLARE	@tblRecordsForHierarchyUpdate TABLE
				(
				  RN INT,
				  Id INT,
				  ParentId INT
				)
					DECLARE	@tblTempRecordsForHierarchyUpdate TABLE
				(
				  Lvl INT,
				  Id INT,
				  ParentId INT
				)

					UPDATE	id
					SET		TypeId = i.TypeId
					FROM	dbo.ItemDefinition id
					INNER JOIN INSERTED i ON i.Id = id.Id

					INSERT	INTO @tblRecordsForHierarchyUpdate
							SELECT	ROW_NUMBER() OVER ( ORDER BY i.Node.GetLevel() ) AS RN,
									i.Id,
									i.ParentId
							FROM	INSERTED i
							INNER JOIN DELETED d ON i.Id = d.Id
													AND ISNULL(i.ParentId, 0) != ISNULL(d.ParentId, 0)

					DECLARE	@CurrentRowNumber INT
					DECLARE	@MaxRowNumber INT
					DECLARE	@Id INT
					DECLARE	@ParentId INT
					DECLARE	@MinNode HIERARCHYID           
					DECLARE	@MaxNode HIERARCHYID     
					DECLARE	@LeftChild HIERARCHYID
					DECLARE	@RightChild HIERARCHYID
					DECLARE	@FirstRelative HIERARCHYID       

					WHILE ( EXISTS ( SELECT TOP 1
											Id
									 FROM	@tblRecordsForHierarchyUpdate ) ) 
						BEGIN
							SET @CurrentRowNumber = NULL
							SET @MaxRowNumber = NULL

							SELECT	@CurrentRowNumber = MIN(RN),
									@MaxRowNumber = MAX(RN)
							FROM	@tblRecordsForHierarchyUpdate

							WHILE ( @CurrentRowNumber <= @MaxRowNumber ) 
								BEGIN 
									SET @Id = NULL
									SET @ParentId = NULL

									SELECT	@Id = Id,
											@ParentId = ParentId
									FROM	@tblRecordsForHierarchyUpdate
									WHERE	RN = @CurrentRowNumber
		      
									SET @MinNode = NULL
									SET @MaxNode = NULL
  
									--ovdje dohvaćamo min i max node od svih childova za trenutnog parenta
									IF @ParentId IS NOT NULL 
										BEGIN         
											SELECT	@MinNode = MIN(i.Node),
													@MaxNode = MAX(i.Node)
											FROM	dbo.ItemDefinition i
											WHERE	i.ParentId = @ParentId
													--ne želimo da uzima u obzir node od itema kojem će se nod tek updatati
													AND NOT EXISTS ( SELECT	Id
																	 FROM	@tblRecordsForHierarchyUpdate
																	 WHERE	Id = i.Id )
										END
									ELSE 
										BEGIN
											SELECT	@MinNode = MIN(i.Node),
													@MaxNode = MAX(i.Node)
											FROM	dbo.ItemDefinition i
											WHERE	i.ParentId IS NULL 
													--ne želimo da uzima u obzir node od itema kojem će se nod tek updatati
													AND NOT EXISTS ( SELECT	Id
																	 FROM	@tblRecordsForHierarchyUpdate
																	 WHERE	Id = i.Id )
										END 

									SET @LeftChild = NULL
									SET @RightChild = NULL
                    
									-- ako je MinNode null to znači da pod parentom ili rootom nema itema, u protivnom postavlja left ili right ovisno s koje strane ima mjesta
									IF @MinNode IS NOT NULL 
										BEGIN
											SET @FirstRelative = HIERARCHYID::Parse(@MinNode.GetAncestor(1).ToString() + '1/')
											IF @FirstRelative < @MinNode 
												BEGIN
													SET @RightChild = @MinNode
												END           
											ELSE 
												BEGIN
													SET @LeftChild = @MaxNode
												END           
										END			

									UPDATE	i
									SET		ParentId = @ParentId,
											Node = n.Node,
											ObjectNode = ISNULL(objN.Node, n.Node)
									FROM	dbo.ItemDefinition i
									LEFT JOIN ItemDefinition p ON p.Id = @ParentId
									--dohvaća Node
									CROSS APPLY ( SELECT	CASE WHEN p.Id IS NULL THEN HIERARCHYID::GetRoot().GetDescendant(@LeftChild, @RightChild)
																 ELSE p.Node.GetDescendant(@LeftChild, @RightChild)
															END AS Node
												) n
									--dohvaća ObjectNode
									OUTER APPLY ( SELECT TOP 1
															o.Node
												  FROM		dbo.ItemDefinition o
												  WHERE		ISNULL(i.TypeId, 0) NOT IN ( " + objectIds + @" )
															AND o.TypeId IN ( " + objectIds + @" )
															AND n.Node.IsDescendantOf(o.Node) = 1
												  ORDER BY	o.Node DESC
												) objN
									WHERE	i.Id = @Id

									--updatamo sve nodove za child item data tablice
									UPDATE	id
									SET		Node = i.Node,
											ObjectNode = i.ObjectNode
									FROM	dbo.ItemDefinitionDataInt id
									INNER JOIN ItemDefinition i ON id.ItemDefinitionId = i.Id
									WHERE	i.Id = @Id

									UPDATE	id
									SET		Node = i.Node,
											ObjectNode = i.ObjectNode
									FROM	dbo.ItemDefinitionDataDecimal id
									INNER JOIN ItemDefinition i ON id.ItemDefinitionId = i.Id
									WHERE	i.Id = @Id

									UPDATE	id
									SET		Node = i.Node,
											ObjectNode = i.ObjectNode
									FROM	dbo.ItemDefinitionDataString id
									INNER JOIN ItemDefinition i ON id.ItemDefinitionId = i.Id
									WHERE	i.Id = @Id

									UPDATE	id
									SET		Node = i.Node,
											ObjectNode = i.ObjectNode
									FROM	dbo.ItemDefinitionDataDateTime id
									INNER JOIN ItemDefinition i ON id.ItemDefinitionId = i.Id
									WHERE	i.Id = @Id

									--dohvaća sve childove jedan nivo niže za trenutni item i inserta ih u privremenu tablicu te ponovno prolazi kroz petlju za svaki od njih
									INSERT	INTO @tblTempRecordsForHierarchyUpdate
											SELECT	id.Node.GetLevel(),
													id.Id,
													@Id
											FROM	dbo.ItemDefinition id
											WHERE	id.ParentId = @Id

									DELETE	FROM @tblRecordsForHierarchyUpdate
									WHERE	RN = @CurrentRowNumber

									SET @CurrentRowNumber = @CurrentRowNumber + 1
								END 

							DELETE	FROM @tblRecordsForHierarchyUpdate
							INSERT	INTO @tblRecordsForHierarchyUpdate
									SELECT	ROW_NUMBER() OVER ( ORDER BY i.Lvl ) AS RN,
											i.Id,
											i.ParentId
									FROM	@tblTempRecordsForHierarchyUpdate i
							DELETE	FROM @tblTempRecordsForHierarchyUpdate
						END
				END");

            context.Database.ExecuteSqlCommand(
            @"CREATE TRIGGER [dbo].[ItemDefinitionInsteadOfDelete] ON [dbo].[ItemDefinition]
					INSTEAD OF DELETE
				AS
				BEGIN
					SET ANSI_WARNINGS OFF
					SET NOCOUNT ON 

					DELETE	i
					FROM	DELETED d
					INNER JOIN dbo.ItemDefinition i ON i.Node.IsDescendantOf(d.Node) = 1
											 OR d.Id = i.Id

					DELETE	i
					FROM	DELETED d
					INNER JOIN dbo.ItemDefinitionDataDateTime i ON i.Node.IsDescendantOf(d.Node) = 1

					DELETE	i
					FROM	DELETED d
					INNER JOIN dbo.ItemDefinitionDataDecimal i ON i.Node.IsDescendantOf(d.Node) = 1

					DELETE	i
					FROM	DELETED d
					INNER JOIN dbo.ItemDefinitionDataInt i ON i.Node.IsDescendantOf(d.Node) = 1

					DELETE	i
					FROM	DELETED d
					INNER JOIN dbo.ItemDefinitionDataString i ON i.Node.IsDescendantOf(d.Node) = 1
				END");
            #endregion
        }

        public static void FillData(WebShopDb context)
        {
            #region ListDefinitions
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.PredefinedList,
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Vrsta lista" },
					new ItemDefinitionDataString { TypeId = 2, Value = "Type list" } 
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						    StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Stambeni objekat" },
							new ItemDefinitionDataString { TypeId = 2, Value = "Apartment" }
						}
					},
					new ItemDefinition { 
        					StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Kuća" },
							new ItemDefinitionDataString { TypeId = 2, Value = "House" }
						}
					}
				}
            });
            #endregion

            context.SaveChanges();

            var vrstaList = (from i in context.ItemDefinition
                             join d in context.ItemDefinitionDataString on i.Id equals d.ItemDefinitionId
                             where i.TypeId == (int)ItemDefinitionTypeEnum.PredefinedList && d.Value == "Vrsta lista"
                             select i).First();

            #region AttributeDefinitions
            #region FixedAttributes
            //Department icon
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.DateTime }, 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDeleteNotAllowed, Value = 1 } ,
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDataTypeChangeNotAllowed, Value = 1 } ,
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingHidden, Value = 1 } ,
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeKey, Value = (int)AttributeKeyEnum.DateTimeEntry },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString {TypeId=1,  Value = "Datum vrijeme unosa" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString {  TypeId=1, Value = "Datum vrijeme unosa" },
						}
					}
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Bool }, 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDeleteNotAllowed, Value = 1 } ,
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDataTypeChangeNotAllowed, Value = 1 } ,
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeKey, Value = (int)AttributeKeyEnum.Top },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString {TypeId=1,  Value = "Top" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString {  TypeId=1, Value = "Top" },
						}
					}
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.String }, 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDeleteNotAllowed, Value = 1 } ,
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDataTypeChangeNotAllowed, Value = 1 } ,
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingHidden, Value = 1 } ,
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeKey, Value = (int)AttributeKeyEnum.DepartmentIcon },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Sličica" },
					new ItemDefinitionDataString { TypeId = 2, Value = "Icon" }
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Sličica" },
							new ItemDefinitionDataString { TypeId = 2, Value = "Icon" }
						}
					}
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Bool } ,
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDeleteNotAllowed, Value = 1 } ,
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDataTypeChangeNotAllowed, Value = 1 } ,
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeKey, Value = (int)AttributeKeyEnum.Premium },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Premium" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Premium" },
						}
					}
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Int },
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDeleteNotAllowed, Value = 1 } ,
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDataTypeChangeNotAllowed, Value = 1 } ,
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingHidden, Value = 1 } ,
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeKey, Value = (int)AttributeKeyEnum.Template },
                },
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Predložak" },
					new ItemDefinitionDataString { TypeId = 2, Value = "Template" }
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Reference },
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeReferenceSystemListId, Value = (int)AttributeDataSystemListReferenceEnum.Partner } ,
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDeleteNotAllowed, Value = 1 },
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDataTypeChangeNotAllowed, Value = 1 } ,
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeKey, Value = (int)AttributeKeyEnum.Partner },
                },
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Partner" },
					new ItemDefinitionDataString { TypeId = 2, Value = "Partner" }
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Partner" },
							new ItemDefinitionDataString { TypeId = 2, Value = "Partner" }
						}
					},
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Reference },
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeReferenceSystemListId, Value = (int)AttributeDataSystemListReferenceEnum.County } ,
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDeleteNotAllowed, Value = 1 },
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDataTypeChangeNotAllowed, Value = 1 } ,
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeKey, Value = (int)AttributeKeyEnum.County },
                },
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Županija" },
					new ItemDefinitionDataString { TypeId = 2, Value = "County" }
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Županija" },
							new ItemDefinitionDataString { TypeId = 2, Value = "County" }
						}
					},
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Reference },
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeReferenceSystemListId, Value = (int)AttributeDataSystemListReferenceEnum.DistrictCity } ,
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDataTypeChangeNotAllowed, Value = 1 } ,
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDeleteNotAllowed, Value = 1 },
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeKey, Value = (int)AttributeKeyEnum.DistrictCity },
                },
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Općina/Grad" },
					new ItemDefinitionDataString { TypeId = 2, Value = "District/City" }
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Općina/Grad" },
							new ItemDefinitionDataString { TypeId = 2, Value = "District/City" }
						}
					},
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.DateTime },
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingHidden, Value = 1 },
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDeleteNotAllowed, Value = 1 },
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDataTypeChangeNotAllowed, Value = 1 } ,
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeKey, Value = (int)AttributeKeyEnum.DateTimeChange },
                },
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Datum promjene" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Datum promjene" },
						}
					},
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Bool },
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDeleteNotAllowed, Value = 1 },
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDataTypeChangeNotAllowed, Value = 1 } ,
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeKey, Value = (int)AttributeKeyEnum.Show },
                },
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Prikaži" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Prikaži" },
						}
					},
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Int },
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingHidden, Value = 1 },
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDeleteNotAllowed, Value = 1 },
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDataTypeChangeNotAllowed, Value = 1 } ,
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeKey, Value = (int)AttributeKeyEnum.PartnerChangedBy },
                },
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "PartnerId promjena" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "PartnerId promjena" },
						}
					},
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Int },
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingHidden, Value = 1 },
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDeleteNotAllowed, Value = 1 },
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDataTypeChangeNotAllowed, Value = 1 } ,
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeKey, Value = (int)AttributeKeyEnum.UserChangedBy },
                },
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "UserId promjena" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "UserId promjena" },
						}
					},
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Decimal } ,
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDeleteNotAllowed, Value = 1 },
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDataTypeChangeNotAllowed, Value = 1 } ,
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeKey, Value = (int)AttributeKeyEnum.Price },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Cijena" },
					new ItemDefinitionDataString { TypeId = 2, Value = "Price" }
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Cijena" },
							new ItemDefinitionDataString { TypeId = 2, Value = "Price" }
						}
					},
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionValueFormat,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { Value = "#,##0.00 kn" }
						}
					}
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.String } ,
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDeleteNotAllowed, Value = 1 },
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDataTypeChangeNotAllowed, Value = 1 } ,
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeKey, Value = (int)AttributeKeyEnum.Name },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Naziv" },
					new ItemDefinitionDataString { TypeId = 2, Value = "Name" }
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Naziv" },
							new ItemDefinitionDataString { TypeId = 2, Value = "Name" }
						}
					}
				}
            });

            //Image attribute
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Image } ,
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDeleteNotAllowed, Value = 1 },
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDataTypeChangeNotAllowed, Value = 1 } ,
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeKey, Value = (int)AttributeKeyEnum.Image },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Slike" },
					new ItemDefinitionDataString { TypeId = 2, Value = "Images" }
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Slike" },
							new ItemDefinitionDataString { TypeId = 2, Value = "Images" }
						}
					}
				}
            });

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
            #endregion

            #region NEKRETNINE
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Coordinates },
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDeleteNotAllowed, Value = 1 },
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDataTypeChangeNotAllowed, Value = 1 } ,
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeKey, Value = (int)AttributeKeyEnum.Location },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Koordinate" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Koordinate" },
						}
					},
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
						IntData = new ObservableCollection<S.IItemData<int>> { 
							new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Decimal },
							new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeKey, Value = (int)AttributeKeyEnum.Longitude },
						},
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Longitude" },
						},
						Children = new ObservableCollection<S.IItem>{
							new ItemDefinition { 
								TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
								StringData = new ObservableCollection<S.IItemData<string>> { 
									new ItemDefinitionDataString { TypeId = 1, Value = "Longitude" },
								}
							}
						}
					},
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
						IntData = new ObservableCollection<S.IItemData<int>> { 
							new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Decimal },
							new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeKey, Value = (int)AttributeKeyEnum.Latitude },
						},
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Latitude" },
						},
						Children = new ObservableCollection<S.IItem>{
							new ItemDefinition { 
								TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
								StringData = new ObservableCollection<S.IItemData<string>> { 
									new ItemDefinitionDataString { TypeId = 1, Value = "Latitude" },
								}
							}
						}
					}
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.DateTime },
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDeleteNotAllowed, Value = 1 },
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDataTypeChangeNotAllowed, Value = 1 } ,
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeKey, Value = (int)AttributeKeyEnum.AuctionDate },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Datum dražbe" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Datum dražbe" },
						}
					}
				}
            });

            #region STAN
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.String},
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Tip stana" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Tip stana" },
						}
					}
				}
            });
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Int},
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Površina stana" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Površina stana" },
						}
					}
				}
            });
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.String },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Adresa prodaje" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Adresa prodaje" },
						}
					}
				}
            });

            #endregion
            #region KUĆE
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.String },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Adresa" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Adresa" },
						}
					}
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.String },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Tip kuće" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Tip kuće" },
						}
					}
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Bool },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Okućnica" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Okućnica" },
						}
					}
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Int },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Površina kuće" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Površina kuće" },
						}
					}
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Int },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Broj soba" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Broj soba" },
						}
					}
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Int },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Broj etaža" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Broj etaža" },
						}
					}
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Int },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Broj katova" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Broj katova" },
						}
					}
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Bool },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Podrum" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Podrum" },
						}
					}
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Int },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Površina podruma" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Površina podruma" },
						}
					}
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Int },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Površina okućnice" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Površina okućnice" },
						}
					}
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Int },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Broj parkirnih mjesta" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Broj parkirnih mjesta" },
						}
					}
				}
            });

            #region GRUPA1
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Bool },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Novogradnja" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Novogradnja" },
						}
					}
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Int },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Godina izgradnje" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Godina izgradnje" },
						}
					}
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Bool },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Godina zadnje adaptacije" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Godina zadnje adaptacije" },
						}
					}
				}
            });
            #endregion

            #region GRUPA2
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.String },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Komunalije" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Komunalije" },
						}
					}
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.String },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Dozvole" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Dozvole" },
						}
					}
				}
            });
            #endregion

            #region GROUP3
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.String },
	                new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeSettingDeleteNotAllowed, Value = 1 },
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeKey, Value = (int)AttributeKeyEnum.Description },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Opis oglasa" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Opis oglasa" },
						}
					}
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.String },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Broj predmeta (stečaj/ovrha)" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Broj predmeta (stečaj/ovrha)" },
						}
					}
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.String },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Razgledavanje predmeta" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Razgledavanje predmeta" },
						}
					}
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.String },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Detalji prodaje" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Detalji prodaje" },
						}
					}
				}
            });
            #endregion
            #endregion
            #region APARTMANI
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.String },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Tip apartmana" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Tip apartmana" },
						}
					}
				}
            });
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Int },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Površina apartmana" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Površina apartmana" },
						}
					}
				}
            });
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Int },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Kat" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Kat" },
						}
					}
				}
            });
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Bool },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Terasa" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Terasa" },
						}
					}
				}
            });
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Int },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Površina terase" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Površina terase" },
						}
					}
				}
            });
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Bool },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Vrt" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Vrt" },
						}
					}
				}
            });
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Int },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Površina vrta" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Površina vrta" },
						}
					}
				}
            });
            #endregion
            #region POSLOVNI PROSTORI I OBJEKTI
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.String },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Tip poslovnog prostora" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Tip poslovnog prostora" },
						}
					}
				}
            });
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Int },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Površina" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Površina" },
						}
					}
				}
            });
            #endregion
            #region GRAĐEVINSKA ZEMLJIŠTA
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.String },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Namjena" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Namjena" },
						}
					}
				}
            });
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Int },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Površina zemljišta" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Površina zemljišta" },
						}
					}
				}
            });
            #endregion

            #region OSOBNA VOZILA
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Int },
                },
                StringData = new ObservableCollection<S.IItemData<string>> { 
                    new ItemDefinitionDataString { TypeId = 1, Value = "Snaga motora" },
                },
                Children = new ObservableCollection<S.IItem>{
                    new ItemDefinition { 
                        TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
                        StringData = new ObservableCollection<S.IItemData<string>> { 
                            new ItemDefinitionDataString { TypeId = 1, Value = "Snaga motora" },
                        }
                    }
                }
            });
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.String },
                },
                StringData = new ObservableCollection<S.IItemData<string>> { 
                    new ItemDefinitionDataString { TypeId = 1, Value = "Vrsta vozila" },
                },
                Children = new ObservableCollection<S.IItem>{
                    new ItemDefinition { 
                        TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
                        StringData = new ObservableCollection<S.IItemData<string>> { 
                            new ItemDefinitionDataString { TypeId = 1, Value = "Vrsta vozila" },
                        }
                    }
                }
            });
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.String },
                },
                StringData = new ObservableCollection<S.IItemData<string>> { 
                    new ItemDefinitionDataString { TypeId = 1, Value = "Marka" },
                },
                Children = new ObservableCollection<S.IItem>{
                    new ItemDefinition { 
                        TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
                        StringData = new ObservableCollection<S.IItemData<string>> { 
                            new ItemDefinitionDataString { TypeId = 1, Value = "Marka" },
                        }
                    }
                }
            });
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.String },
                },
                StringData = new ObservableCollection<S.IItemData<string>> { 
                    new ItemDefinitionDataString { TypeId = 1, Value = "Model" },
                },
                Children = new ObservableCollection<S.IItem>{
                    new ItemDefinition { 
                        TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
                        StringData = new ObservableCollection<S.IItemData<string>> { 
                            new ItemDefinitionDataString { TypeId = 1, Value = "Model" },
                        }
                    }
                }
            });
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.String },
                },
                StringData = new ObservableCollection<S.IItemData<string>> { 
                    new ItemDefinitionDataString { TypeId = 1, Value = "Tip vozila" },
                },
                Children = new ObservableCollection<S.IItem>{
                    new ItemDefinition { 
                        TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
                        StringData = new ObservableCollection<S.IItemData<string>> { 
                            new ItemDefinitionDataString { TypeId = 1, Value = "Tip vozila" },
                        }
                    }
                }
            });
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Int },
                },
                StringData = new ObservableCollection<S.IItemData<string>> { 
                    new ItemDefinitionDataString { TypeId = 1, Value = "Godina proizvodnje" },
                },
                Children = new ObservableCollection<S.IItem>{
                    new ItemDefinition { 
                        TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
                        StringData = new ObservableCollection<S.IItemData<string>> { 
                            new ItemDefinitionDataString { TypeId = 1, Value = "Godina proizvodnje" },
                        }
                    }
                }
            });
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.String },
                },
                StringData = new ObservableCollection<S.IItemData<string>> { 
                    new ItemDefinitionDataString { TypeId = 1, Value = "Motor" },
                },
                Children = new ObservableCollection<S.IItem>{
                    new ItemDefinition { 
                        TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
                        StringData = new ObservableCollection<S.IItemData<string>> { 
                            new ItemDefinitionDataString { TypeId = 1, Value = "Motor" },
                        }
                    }
                }
            });
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Int },
                },
                StringData = new ObservableCollection<S.IItemData<string>> { 
                    new ItemDefinitionDataString { TypeId = 1, Value = "Radni obujam" },
                },
                Children = new ObservableCollection<S.IItem>{
                    new ItemDefinition { 
                        TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
                        StringData = new ObservableCollection<S.IItemData<string>> { 
                            new ItemDefinitionDataString { TypeId = 1, Value = "Radni obujam" },
                        }
                    }
                }
            });
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Int },
                },
                StringData = new ObservableCollection<S.IItemData<string>> { 
                    new ItemDefinitionDataString { TypeId = 1, Value = "Prijeđeni kilometri" },
                },
                Children = new ObservableCollection<S.IItem>{
                    new ItemDefinition { 
                        TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
                        StringData = new ObservableCollection<S.IItemData<string>> { 
                            new ItemDefinitionDataString { TypeId = 1, Value = "Prijeđeni kilometri" },
                        }
                    }
                }
            });
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Int },
                },
                StringData = new ObservableCollection<S.IItemData<string>> { 
                    new ItemDefinitionDataString { TypeId = 1, Value = "Registriran do" },
                },
                Children = new ObservableCollection<S.IItem>{
                    new ItemDefinition { 
                        TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
                        StringData = new ObservableCollection<S.IItemData<string>> { 
                            new ItemDefinitionDataString { TypeId = 1, Value = "Registriran do" },
                        }
                    }
                }
            });
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.String },
                },
                StringData = new ObservableCollection<S.IItemData<string>> { 
                    new ItemDefinitionDataString { TypeId = 1, Value = "Mjenjač" },
                },
                Children = new ObservableCollection<S.IItem>{
                    new ItemDefinition { 
                        TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
                        StringData = new ObservableCollection<S.IItemData<string>> { 
                            new ItemDefinitionDataString { TypeId = 1, Value = "Mjenjač" },
                        }
                    }
                }
            });
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Int },
                },
                StringData = new ObservableCollection<S.IItemData<string>> { 
                    new ItemDefinitionDataString { TypeId = 1, Value = "Broj stupnjeva" },
                },
                Children = new ObservableCollection<S.IItem>{
                    new ItemDefinition { 
                        TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
                        StringData = new ObservableCollection<S.IItemData<string>> { 
                            new ItemDefinitionDataString { TypeId = 1, Value = "Broj stupnjeva" },
                        }
                    }
                }
            });
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.String },
                },
                StringData = new ObservableCollection<S.IItemData<string>> { 
                    new ItemDefinitionDataString { TypeId = 1, Value = "Očuvanost vozila" },
                },
                Children = new ObservableCollection<S.IItem>{
                    new ItemDefinition { 
                        TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
                        StringData = new ObservableCollection<S.IItemData<string>> { 
                            new ItemDefinitionDataString { TypeId = 1, Value = "Očuvanost vozila" },
                        }
                    }
                }
            });
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Int },
                },
                StringData = new ObservableCollection<S.IItemData<string>> { 
                    new ItemDefinitionDataString { TypeId = 1, Value = "Garancija" },
                },
                Children = new ObservableCollection<S.IItem>{
                    new ItemDefinition { 
                        TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
                        StringData = new ObservableCollection<S.IItemData<string>> { 
                            new ItemDefinitionDataString { TypeId = 1, Value = "Garancija" },
                        }
                    }
                }
            });
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Bool },
                },
                StringData = new ObservableCollection<S.IItemData<string>> { 
                    new ItemDefinitionDataString { TypeId = 1, Value = "Servisna knjiga" },
                },
                Children = new ObservableCollection<S.IItem>{
                    new ItemDefinition { 
                        TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
                        StringData = new ObservableCollection<S.IItemData<string>> { 
                            new ItemDefinitionDataString { TypeId = 1, Value = "Servisna knjiga" },
                        }
                    }
                }
            });
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.String },
                },
                StringData = new ObservableCollection<S.IItemData<string>> { 
                    new ItemDefinitionDataString { TypeId = 1, Value = "Vlasnik" },
                },
                Children = new ObservableCollection<S.IItem>{
                    new ItemDefinition { 
                        TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
                        StringData = new ObservableCollection<S.IItemData<string>> { 
                            new ItemDefinitionDataString { TypeId = 1, Value = "Vlasnik" },
                        }
                    }
                }
            });
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Bool },
                },
                StringData = new ObservableCollection<S.IItemData<string>> { 
                    new ItemDefinitionDataString { TypeId = 1, Value = "Garažiran" },
                },
                Children = new ObservableCollection<S.IItem>{
                    new ItemDefinition { 
                        TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
                        StringData = new ObservableCollection<S.IItemData<string>> { 
                            new ItemDefinitionDataString { TypeId = 1, Value = "Garažiran" },
                        }
                    }
                }
            });
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
                    new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Bool },
                },
                StringData = new ObservableCollection<S.IItemData<string>> { 
                    new ItemDefinitionDataString { TypeId = 1, Value = "Oprema vozila" },
                },
                Children = new ObservableCollection<S.IItem>{
                    new ItemDefinition { 
                        TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
                        StringData = new ObservableCollection<S.IItemData<string>> { 
                            new ItemDefinitionDataString { TypeId = 1, Value = "Oprema vozila" },
                        }
                    }
                }
            });
            #endregion

            #region OSOBNA PLOVILA
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.String },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Vrsta plovila" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Vrsta plovila" },
						}
					}
				}
            });
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Int },
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Dužina" },
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Dužina" },
						}
					}
				}
            });
            #endregion

            #endregion

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { 
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Reference },
					new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeReferencePredefinedListId, Value = vrstaList.Id }
				},
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Vrsta" },
					new ItemDefinitionDataString { TypeId = 2, Value = "Type" }
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Vrsta" },
							new ItemDefinitionDataString { TypeId = 2, Value = "Type" }
						}
					}
				}
            });

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinition,
                IntData = new ObservableCollection<S.IItemData<int>> { new ItemDefinitionDataInt { TypeId = (int)ItemDefinitionDataTypeEnum.AttributeDataType, Value = (int)AttributeDataTypeEnum.Decimal } },
                StringData = new ObservableCollection<S.IItemData<string>> { 
					new ItemDefinitionDataString { TypeId = 1, Value = "Udaljenost od tramvaja" },
					new ItemDefinitionDataString { TypeId = 2, Value = "Distance from tram" }
				},
                Children = new ObservableCollection<S.IItem>{
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionDisplayValue,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "Udaljenost od tramvaja" },
							new ItemDefinitionDataString { TypeId = 2, Value = "Distance from tram" }
						}
					},
					new ItemDefinition { 
						TypeId = (int)ItemDefinitionTypeEnum.AttributeDefinitionValueFormat,
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDefinitionDataString { TypeId = 1, Value = "0" },
							new ItemDefinitionDataString { TypeId = 2, Value = "0.00" }
						}
					}
				}
            });

            context.SaveChanges();
            #endregion

            var atts = (from ids in context.ItemDefinitionDataString
                        join i in context.ItemDefinition on ids.ItemDefinitionId equals i.Id
                        where i.TypeId == (int)ItemDefinitionTypeEnum.AttributeDefinition
                                && ids.TypeId == 1
                        select new { Key = ids.Value, Value = i.Id }).ToDictionary(d => d.Key);

            #region AttributeTemplates
            #region NEKRETNINE
            #region STAN
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplate,
                StringData = new ObservableCollection<S.IItemData<string>>() { new ItemDefinitionDataString { TypeId = 1, Value = "Stan" } },
                Children = new ObservableCollection<S.IItem>{
					                       new ItemDefinition{
						                       TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						                       IntData = new ObservableCollection<S.IItemData<int>>{
							                   new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Partner"].Value },
							                   new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
						                    }
					                    },
					                    new ItemDefinition{
						                    TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						                    IntData = new ObservableCollection<S.IItemData<int>>{new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Naziv"].Value },
					                        new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
						                    }
					                    },
					                    new ItemDefinition{
						                    TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						                    IntData = new ObservableCollection<S.IItemData<int>>{
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Premium"].Value },
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
						                    }
					                    },
					                    new ItemDefinition{
						                    TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						                    IntData = new ObservableCollection<S.IItemData<int>>{
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Cijena"].Value },
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
						                    }
					                    },
                                        new ItemDefinition{
								            TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								            IntData = new ObservableCollection<S.IItemData<int>>{
									            new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Datum dražbe"].Value },
									            new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
								            }
							            },
                                        new ItemDefinition{
                                            TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
                                            IntData = new ObservableCollection<S.IItemData<int>>{
                                                new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Slike"].Value },
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
                                            }
                                        },
						                new ItemDefinition{
							                TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
							                IntData = new ObservableCollection<S.IItemData<int>>{
								                new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Koordinate"].Value },
								                new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
							                }
						                },
					new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "INFORMACIJE O PREDMETU " }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
						},
						 Children= new ObservableCollection<S.IItem>{

							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Tip stana"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Županija"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Općina/Grad"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								},
								Children = new ObservableCollection<S.IItem>{
									 new ItemDefinition{
										 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateConstraint,
										 IntData = new ObservableCollection<S.IItemData<int>>{
											 new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateConstraintForeignKeyAttribute },
											 new ItemDefinitionDataInt{ Value=atts["Županija"].Value }
										 },
									},
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Adresa"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								},
								Children = new ObservableCollection<S.IItem>{
									new ItemDefinition{
										TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateConstraint,
										IntData = new ObservableCollection<S.IItemData<int>>{
											new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateConstraintMinEntry },
											new ItemDefinitionDataInt{ Value=1 }
										},
								   },
								   new ItemDefinition{
										TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateConstraint,
										IntData = new ObservableCollection<S.IItemData<int>>{
										   new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateConstraintMaxEntry },
										   new ItemDefinitionDataInt{ Value=1 }
										},
								   }
							   }
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Površina stana"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj soba"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Kat"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Terasa"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Novogradnja"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=9 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Godina izgradnje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=10 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj parkirnih mjesta"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=11 },
								},
							},
						 },
					},
					new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "DODATNE INFORMACIJE" }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=9 },
						},
						 Children= new ObservableCollection<S.IItem>{
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Godina zadnje adaptacije"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Površina terase"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj etaža"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Vrt"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Površina vrta"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Komunalije"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Dozvole"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Opis oglasa"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
								}
							},
						}
					},
					new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "INFORMACIJE O PRODAJI" }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=10 },
						},
						 Children= new ObservableCollection<S.IItem>{
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj predmeta (stečaj/ovrha)"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Razgledavanje predmeta"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Adresa prodaje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Detalji prodaje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								}
							},
						 }
					},
				}

            });
            #endregion

            #region KUĆE
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplate,
                StringData = new ObservableCollection<S.IItemData<string>>() { new ItemDefinitionDataString { TypeId = 1, Value = "Kuće" } },
                Children = new ObservableCollection<S.IItem>{
 					new ItemDefinition{
						    TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						    IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Partner"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
						}
					},
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Naziv"].Value },
					    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
						}
					},
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Premium"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
						}
					},
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Cijena"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
						}
					},
                    new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Datum dražbe"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
						}
					},
                       new ItemDefinition{
                        TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
                        IntData = new ObservableCollection<S.IItemData<int>>{
                            new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Slike"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
                        }
                    },
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Koordinate"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
						}
					},
                 
                 new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "INFORMACIJE O PREDMETU " }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
						},
						 Children= new ObservableCollection<S.IItem>{

							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Tip kuće"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Županija"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Općina/Grad"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								},
								Children = new ObservableCollection<S.IItem>{
									 new ItemDefinition{
										 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateConstraint,
										 IntData = new ObservableCollection<S.IItemData<int>>{
											 new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateConstraintForeignKeyAttribute },
											 new ItemDefinitionDataInt{ Value=atts["Županija"].Value }
										 },
									},
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Adresa"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								},
								Children = new ObservableCollection<S.IItem>{
									new ItemDefinition{
										TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateConstraint,
										IntData = new ObservableCollection<S.IItemData<int>>{
											new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateConstraintMinEntry },
											new ItemDefinitionDataInt{ Value=1 }
										},
								   },
								   new ItemDefinition{
										TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateConstraint,
										IntData = new ObservableCollection<S.IItemData<int>>{
										   new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateConstraintMaxEntry },
										   new ItemDefinitionDataInt{ Value=1 }
										},
								   }
							   }
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Površina kuće"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj soba"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj katova"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Okućnica"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Novogradnja"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=9 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Godina izgradnje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=10 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj parkirnih mjesta"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=11 },
								},
							},
						 },
					},
					new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "DODATNE INFORMACIJE" }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=9 },
						},
						 Children= new ObservableCollection<S.IItem>{
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Godina zadnje adaptacije"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Površina terase"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj etaža"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Podrum"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Površina podruma"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Komunalije"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Dozvole"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Opis oglasa"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
								}
							},
						}
					},
                    	new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "INFORMACIJE O PRODAJI" }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=10 },
						},
						 Children= new ObservableCollection<S.IItem>{
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj predmeta (stečaj/ovrha)"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Razgledavanje predmeta"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Adresa prodaje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Detalji prodaje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								}
							},
						 }
					},
				}
            });
            #endregion

            #region APARTMANI
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplate,
                StringData = new ObservableCollection<S.IItemData<string>>() { new ItemDefinitionDataString { TypeId = 1, Value = "Apartmani" } },
                Children = new ObservableCollection<S.IItem>{
 					new ItemDefinition{
						    TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						    IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Partner"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
						}
					},
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Naziv"].Value },
					    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
						}
					},
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Premium"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
						}
					},
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Cijena"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
						}
					},
                    new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Datum dražbe"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
						}
					},
                             new ItemDefinition{
                        TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
                        IntData = new ObservableCollection<S.IItemData<int>>{
                            new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Slike"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
                        }
                    },
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Koordinate"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
						}
					},
           
                 new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "INFORMACIJE O PREDMETU " }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
						},
						 Children= new ObservableCollection<S.IItem>{

							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Tip apartmana"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Županija"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Općina/Grad"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								},
								Children = new ObservableCollection<S.IItem>{
									 new ItemDefinition{
										 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateConstraint,
										 IntData = new ObservableCollection<S.IItemData<int>>{
											 new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateConstraintForeignKeyAttribute },
											 new ItemDefinitionDataInt{ Value=atts["Županija"].Value }
										 },
									},
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Adresa"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								},
								Children = new ObservableCollection<S.IItem>{
									new ItemDefinition{
										TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateConstraint,
										IntData = new ObservableCollection<S.IItemData<int>>{
											new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateConstraintMinEntry },
											new ItemDefinitionDataInt{ Value=1 }
										},
								   },
								   new ItemDefinition{
										TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateConstraint,
										IntData = new ObservableCollection<S.IItemData<int>>{
										   new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateConstraintMaxEntry },
										   new ItemDefinitionDataInt{ Value=1 }
										},
								   }
							   }
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Površina apartmana"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj soba"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Kat"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Terasa"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Novogradnja"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=9 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Godina izgradnje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=10 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj parkirnih mjesta"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=11 },
								},
							},
						 },
					},
					new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "DODATNE INFORMACIJE" }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=9 },
						},
						 Children= new ObservableCollection<S.IItem>{
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Godina zadnje adaptacije"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Površina terase"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj etaža"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Vrt"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Površina vrta"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Komunalije"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Dozvole"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Opis oglasa"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
								}
							},
						}
					},
                    	new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "INFORMACIJE O PRODAJI" }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=10 },
						},
						 Children= new ObservableCollection<S.IItem>{
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj predmeta (stečaj/ovrha)"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Razgledavanje predmeta"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Adresa prodaje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Detalji prodaje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								}
							},
						 }
					},
				}
            });
            #endregion

            #region POSLOVNI PROSTORI I OBJEKTI
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplate,
                StringData = new ObservableCollection<S.IItemData<string>>() { new ItemDefinitionDataString { TypeId = 1, Value = "Poslovni prostori i objekti" } },
                Children = new ObservableCollection<S.IItem>{
 					new ItemDefinition{
						    TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						    IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Partner"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
						}
					},
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Naziv"].Value },
					    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
						}
					},
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Premium"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
						}
					},
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Cijena"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
						}
					},
                    new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Datum dražbe"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
						}
					},
                          new ItemDefinition{
                        TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
                        IntData = new ObservableCollection<S.IItemData<int>>{
                            new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Slike"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
                        }
                    },
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Koordinate"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
						}
					},
              
                 new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "INFORMACIJE O PREDMETU " }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
						},
						 Children= new ObservableCollection<S.IItem>{

							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Tip stana"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Županija"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Općina/Grad"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								},
								Children = new ObservableCollection<S.IItem>{
									 new ItemDefinition{
										 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateConstraint,
										 IntData = new ObservableCollection<S.IItemData<int>>{
											 new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateConstraintForeignKeyAttribute },
											 new ItemDefinitionDataInt{ Value=atts["Županija"].Value }
										 },
									},
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Adresa"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								},
								Children = new ObservableCollection<S.IItem>{
									new ItemDefinition{
										TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateConstraint,
										IntData = new ObservableCollection<S.IItemData<int>>{
											new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateConstraintMinEntry },
											new ItemDefinitionDataInt{ Value=1 }
										},
								   },
								   new ItemDefinition{
										TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateConstraint,
										IntData = new ObservableCollection<S.IItemData<int>>{
										   new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateConstraintMaxEntry },
										   new ItemDefinitionDataInt{ Value=1 }
										},
								   }
							   }
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Površina"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj soba"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Kat"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Terasa"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Novogradnja"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=9 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Godina izgradnje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=10 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj parkirnih mjesta"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=11 },
								},
							},
						 },
					},
					new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "DODATNE INFORMACIJE" }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=9 },
						},
						 Children= new ObservableCollection<S.IItem>{
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Godina zadnje adaptacije"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Površina terase"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj etaža"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Komunalije"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Dozvole"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Opis oglasa"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
								}
							},
						}
					},
                    	new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "INFORMACIJE O PRODAJI" }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=10 },
						},
						 Children= new ObservableCollection<S.IItem>{
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj predmeta (stečaj/ovrha)"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Razgledavanje predmeta"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Adresa prodaje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Detalji prodaje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								}
							},
						 }
					},
				}
            });
            #endregion

            #region GRAĐEVINSKA ZEMLJIŠTA

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplate,
                StringData = new ObservableCollection<S.IItemData<string>>() { new ItemDefinitionDataString { TypeId = 1, Value = "Građevinska zemljišta" } },
                Children = new ObservableCollection<S.IItem>{
 					new ItemDefinition{
						    TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						    IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Partner"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
						}
					},
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Naziv"].Value },
					    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
						}
					},
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Premium"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
						}
					},
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Cijena"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
						}
					},
                    new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Datum dražbe"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
						}
					},
                     new ItemDefinition{
                        TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
                        IntData = new ObservableCollection<S.IItemData<int>>{
                            new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Slike"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
                        }
                    },
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Koordinate"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
						}
					},
                 new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "INFORMACIJE O PREDMETU " }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
						},
						 Children= new ObservableCollection<S.IItem>{

							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Namjena"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Županija"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Općina/Grad"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								},
								Children = new ObservableCollection<S.IItem>{
									 new ItemDefinition{
										 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateConstraint,
										 IntData = new ObservableCollection<S.IItemData<int>>{
											 new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateConstraintForeignKeyAttribute },
											 new ItemDefinitionDataInt{ Value=atts["Županija"].Value }
										 },
									},
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Adresa"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								},
								Children = new ObservableCollection<S.IItem>{
									new ItemDefinition{
										TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateConstraint,
										IntData = new ObservableCollection<S.IItemData<int>>{
											new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateConstraintMinEntry },
											new ItemDefinitionDataInt{ Value=1 }
										},
								   },
								   new ItemDefinition{
										TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateConstraint,
										IntData = new ObservableCollection<S.IItemData<int>>{
										   new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateConstraintMaxEntry },
										   new ItemDefinitionDataInt{ Value=1 }
										},
								   }
							   }
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Površina zemljišta"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
								}
							},
						 },
					},
					new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "DODATNE INFORMACIJE" }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=9 },
						},
						 Children= new ObservableCollection<S.IItem>{
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Komunalije"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Dozvole"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Opis oglasa"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
						}
					},
                    	new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "INFORMACIJE O PRODAJI" }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=10 },
						},
						 Children= new ObservableCollection<S.IItem>{
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj predmeta (stečaj/ovrha)"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Razgledavanje predmeta"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Adresa prodaje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Detalji prodaje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								}
							},
						 }
					},
				}
            });
            #endregion

            #region POLJOPRIVREDNA ZEMLJIŠTA

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplate,
                StringData = new ObservableCollection<S.IItemData<string>>() { new ItemDefinitionDataString { TypeId = 1, Value = "Poljoprivredna zemljišta" } },
                Children = new ObservableCollection<S.IItem>{
 					new ItemDefinition{
						    TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						    IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Partner"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
						}
					},
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Naziv"].Value },
					    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
						}
					},
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Premium"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
						}
					},
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Cijena"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
						}
					},
                    new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Datum dražbe"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
						}
					},
                    new ItemDefinition{
                        TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
                        IntData = new ObservableCollection<S.IItemData<int>>{
                            new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Slike"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
                        }
                    },
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Koordinate"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
						}
					},
                 new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "INFORMACIJE O PREDMETU " }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
						},
						 Children= new ObservableCollection<S.IItem>{

							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Namjena"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Županija"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Općina/Grad"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								},
								Children = new ObservableCollection<S.IItem>{
									 new ItemDefinition{
										 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateConstraint,
										 IntData = new ObservableCollection<S.IItemData<int>>{
											 new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateConstraintForeignKeyAttribute },
											 new ItemDefinitionDataInt{ Value=atts["Županija"].Value }
										 },
									},
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Adresa"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								},
								Children = new ObservableCollection<S.IItem>{
									new ItemDefinition{
										TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateConstraint,
										IntData = new ObservableCollection<S.IItemData<int>>{
											new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateConstraintMinEntry },
											new ItemDefinitionDataInt{ Value=1 }
										},
								   },
								   new ItemDefinition{
										TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateConstraint,
										IntData = new ObservableCollection<S.IItemData<int>>{
										   new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateConstraintMaxEntry },
										   new ItemDefinitionDataInt{ Value=1 }
										},
								   }
							   }
							},
						 },
					},
					new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "DODATNE INFORMACIJE" }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
						},
						 Children= new ObservableCollection<S.IItem>{
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Komunalije"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Dozvole"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Opis oglasa"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
						}
					},
                    	new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "INFORMACIJE O PRODAJI" }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=9 },
						},
						 Children= new ObservableCollection<S.IItem>{
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj predmeta (stečaj/ovrha)"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Razgledavanje predmeta"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Adresa prodaje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Detalji prodaje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								}
							},
						 }
					},
				}
            });
            #endregion

            #region OSTALO

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplate,
                StringData = new ObservableCollection<S.IItemData<string>>() { new ItemDefinitionDataString { TypeId = 1, Value = "Ostalo" } },
                Children = new ObservableCollection<S.IItem>{
 					new ItemDefinition{
						    TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						    IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Partner"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
						}
					},
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Naziv"].Value },
					    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
						}
					},
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Premium"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
						}
					},
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Cijena"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
						}
					},
                    new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Datum dražbe"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
						}
					},
                     new ItemDefinition{
                        TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
                        IntData = new ObservableCollection<S.IItemData<int>>{
                            new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Slike"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
                        }
                    },
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Koordinate"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
						}
					},
                 new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "INFORMACIJE O PREDMETU " }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
						},
						 Children= new ObservableCollection<S.IItem>{

							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Tip stana"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Županija"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Općina/Grad"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								},
								Children = new ObservableCollection<S.IItem>{
									 new ItemDefinition{
										 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateConstraint,
										 IntData = new ObservableCollection<S.IItemData<int>>{
											 new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateConstraintForeignKeyAttribute },
											 new ItemDefinitionDataInt{ Value=atts["Županija"].Value }
										 },
									},
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Adresa"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								},
								Children = new ObservableCollection<S.IItem>{
									new ItemDefinition{
										TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateConstraint,
										IntData = new ObservableCollection<S.IItemData<int>>{
											new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateConstraintMinEntry },
											new ItemDefinitionDataInt{ Value=1 }
										},
								   },
								   new ItemDefinition{
										TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateConstraint,
										IntData = new ObservableCollection<S.IItemData<int>>{
										   new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateConstraintMaxEntry },
										   new ItemDefinitionDataInt{ Value=1 }
										},
								   }
							   }
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Površina stana"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj soba"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Kat"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Terasa"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Novogradnja"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=9 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Godina izgradnje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=10 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj parkirnih mjesta"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=11 },
								},
							},
						 },
					},
					new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "DODATNE INFORMACIJE" }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=9 },
						},
						 Children= new ObservableCollection<S.IItem>{
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Godina zadnje adaptacije"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Površina terase"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj etaža"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Vrt"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Površina vrta"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Komunalije"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Dozvole"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Opis oglasa"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
								}
							},
						}
					},
                    	new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "INFORMACIJE O PRODAJI" }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=10 },
						},
						 Children= new ObservableCollection<S.IItem>{
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj predmeta (stečaj/ovrha)"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Razgledavanje predmeta"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Adresa prodaje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Detalji prodaje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								}
							},
						 }
					},
				}
            });
            #endregion

            #endregion

            #region MOTORNA VOZILA
            #region OSOBNA VOZILA
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplate,
                StringData = new ObservableCollection<S.IItemData<string>>() { new ItemDefinitionDataString { TypeId = 1, Value = "Osobna vozila" } },
                Children = new ObservableCollection<S.IItem>{
					                       new ItemDefinition{
						                       TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						                       IntData = new ObservableCollection<S.IItemData<int>>{
							                   new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Partner"].Value },
							                   new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
						                    }
					                    },
					                    new ItemDefinition{
						                    TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						                    IntData = new ObservableCollection<S.IItemData<int>>{new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Naziv"].Value },
					                        new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
						                    }
					                    },
					                    new ItemDefinition{
						                    TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						                    IntData = new ObservableCollection<S.IItemData<int>>{
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Premium"].Value },
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
						                    }
					                    },
					                    new ItemDefinition{
						                    TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						                    IntData = new ObservableCollection<S.IItemData<int>>{
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Cijena"].Value },
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
						                    }
					                    },
                                        new ItemDefinition{
								            TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								            IntData = new ObservableCollection<S.IItemData<int>>{
									            new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Datum dražbe"].Value },
									            new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
								            }
							            },
                                        new ItemDefinition{
                                            TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
                                            IntData = new ObservableCollection<S.IItemData<int>>{
                                                new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Slike"].Value },
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
                                            }
                                        },
						                new ItemDefinition{
							                TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
							                IntData = new ObservableCollection<S.IItemData<int>>{
								                new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Koordinate"].Value },
								                new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
							                }
						                },
					new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "INFORMACIJE O PREDMETU " }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
						},
						 Children= new ObservableCollection<S.IItem>{

							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Vrsta vozila"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
    						new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Marka"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Model"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Tip vozila"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Godina proizvodnje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Motor"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Radni obujam"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Snaga motora"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
								}
							},
                            new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Prijeđeni kilometri"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=9 },
								}
							},
                             new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Registriran do"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=10 },
								}
							},
						 },
					},
					new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "DODATNE INFORMACIJE" }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=9 },
						},
						 Children= new ObservableCollection<S.IItem>{
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Mjenjač"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj stupnjeva"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Očuvanost vozila"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Garancija"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Servisna knjiga"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Vlasnik"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Garažiran"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Oprema vozila"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
								}
							},
                            new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Opis oglasa"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
								}
							},
						}
					},
					new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "INFORMACIJE O PRODAJI" }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=10 },
						},
						 Children= new ObservableCollection<S.IItem>{
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj predmeta (stečaj/ovrha)"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Razgledavanje predmeta"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Adresa prodaje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Detalji prodaje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								}
							},
						 }
					},
				}

            });
            #endregion

            #region GOSPODARSKA VOZILA
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplate,
                StringData = new ObservableCollection<S.IItemData<string>>() { new ItemDefinitionDataString { TypeId = 1, Value = "Gospodarska vozila" } },
                Children = new ObservableCollection<S.IItem>{
					                       new ItemDefinition{
						                       TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						                       IntData = new ObservableCollection<S.IItemData<int>>{
							                   new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Partner"].Value },
							                   new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
						                    }
					                    },
					                    new ItemDefinition{
						                    TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						                    IntData = new ObservableCollection<S.IItemData<int>>{new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Naziv"].Value },
					                        new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
						                    }
					                    },
					                    new ItemDefinition{
						                    TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						                    IntData = new ObservableCollection<S.IItemData<int>>{
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Premium"].Value },
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
						                    }
					                    },
					                    new ItemDefinition{
						                    TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						                    IntData = new ObservableCollection<S.IItemData<int>>{
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Cijena"].Value },
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
						                    }
					                    },
                                        new ItemDefinition{
								            TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								            IntData = new ObservableCollection<S.IItemData<int>>{
									            new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Datum dražbe"].Value },
									            new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
								            }
							            },
                                        new ItemDefinition{
                                            TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
                                            IntData = new ObservableCollection<S.IItemData<int>>{
                                                new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Slike"].Value },
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
                                            }
                                        },
						                new ItemDefinition{
							                TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
							                IntData = new ObservableCollection<S.IItemData<int>>{
								                new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Koordinate"].Value },
								                new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
							                }
						                },
					new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "INFORMACIJE O PREDMETU " }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
						},
						 Children= new ObservableCollection<S.IItem>{

							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Vrsta vozila"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
    						new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Marka"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Model"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Tip vozila"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Godina proizvodnje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Motor"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Radni obujam"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Snaga motora"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
								}
							},
                            new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Prijeđeni kilometri"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=9 },
								}
							},
                             new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Registriran do"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=10 },
								}
							},
						 },
					},
					new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "DODATNE INFORMACIJE" }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=9 },
						},
						 Children= new ObservableCollection<S.IItem>{
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Mjenjač"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj stupnjeva"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Očuvanost vozila"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Garancija"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Servisna knjiga"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Vlasnik"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Garažiran"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Oprema vozila"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
								}
							},
                            new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Opis oglasa"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
								}
							},
						}
					},
					new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "INFORMACIJE O PRODAJI" }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=10 },
						},
						 Children= new ObservableCollection<S.IItem>{
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj predmeta (stečaj/ovrha)"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Razgledavanje predmeta"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Adresa prodaje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Detalji prodaje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								}
							},
						 }
					},
				}

            });
            #endregion

            #region MOTORI
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplate,
                StringData = new ObservableCollection<S.IItemData<string>>() { new ItemDefinitionDataString { TypeId = 1, Value = "Motori" } },
                Children = new ObservableCollection<S.IItem>{
					                       new ItemDefinition{
						                       TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						                       IntData = new ObservableCollection<S.IItemData<int>>{
							                   new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Partner"].Value },
							                   new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
						                    }
					                    },
					                    new ItemDefinition{
						                    TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						                    IntData = new ObservableCollection<S.IItemData<int>>{new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Naziv"].Value },
					                        new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
						                    }
					                    },
					                    new ItemDefinition{
						                    TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						                    IntData = new ObservableCollection<S.IItemData<int>>{
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Premium"].Value },
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
						                    }
					                    },
					                    new ItemDefinition{
						                    TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						                    IntData = new ObservableCollection<S.IItemData<int>>{
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Cijena"].Value },
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
						                    }
					                    },
                                        new ItemDefinition{
								            TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								            IntData = new ObservableCollection<S.IItemData<int>>{
									            new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Datum dražbe"].Value },
									            new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
								            }
							            },
                                        new ItemDefinition{
                                            TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
                                            IntData = new ObservableCollection<S.IItemData<int>>{
                                                new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Slike"].Value },
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
                                            }
                                        },
						                new ItemDefinition{
							                TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
							                IntData = new ObservableCollection<S.IItemData<int>>{
								                new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Koordinate"].Value },
								                new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
							                }
						                },
					new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "INFORMACIJE O PREDMETU " }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
						},
						 Children= new ObservableCollection<S.IItem>{

							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Vrsta"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
    						new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Marka"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Model"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Tip vozila"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Godina proizvodnje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Motor"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Radni obujam"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Snaga motora"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
								}
							},
                            new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Prijeđeni kilometri"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=9 },
								}
							},
                             new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Registriran do"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=10 },
								}
							},
						 },
					},
					new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "DODATNE INFORMACIJE" }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=9 },
						},
						 Children= new ObservableCollection<S.IItem>{
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Mjenjač"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj stupnjeva"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Očuvanost vozila"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Garancija"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Servisna knjiga"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Vlasnik"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Garažiran"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Oprema vozila"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
								}
							},
                            new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Opis oglasa"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
								}
							},
						}
					},
					new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "INFORMACIJE O PRODAJI" }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=10 },
						},
						 Children= new ObservableCollection<S.IItem>{
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj predmeta (stečaj/ovrha)"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Razgledavanje predmeta"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Adresa prodaje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Detalji prodaje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								}
							},
						 }
					},
				}

            });
            #endregion

            #region OSTALO

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplate,
                StringData = new ObservableCollection<S.IItemData<string>>() { new ItemDefinitionDataString { TypeId = 1, Value = "Ostalo 1" } },
                Children = new ObservableCollection<S.IItem>{
 					new ItemDefinition{
						    TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						    IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Partner"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
						}
					},
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Naziv"].Value },
					    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
						}
					},
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Premium"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
						}
					},
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Cijena"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
						}
					},
                    new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Datum dražbe"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
						}
					},
                     new ItemDefinition{
                        TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
                        IntData = new ObservableCollection<S.IItemData<int>>{
                            new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Slike"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
                        }
                    },
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Koordinate"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
						}
					},
					new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "DODATNE INFORMACIJE" }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
						},
						 Children= new ObservableCollection<S.IItem>{
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Opis oglasa"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
						}
					},
                    	new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "INFORMACIJE O PRODAJI" }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=9 },
						},
						 Children= new ObservableCollection<S.IItem>{
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj predmeta (stečaj/ovrha)"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Razgledavanje predmeta"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Adresa prodaje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Detalji prodaje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								}
							},
						 }
					},
				}
            });
            #endregion

            #endregion

            #region PLOVILA

            #region OSOBNA PLOVILA
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplate,
                StringData = new ObservableCollection<S.IItemData<string>>() { new ItemDefinitionDataString { TypeId = 1, Value = "Osobna plovila" } },
                Children = new ObservableCollection<S.IItem>{
					                       new ItemDefinition{
						                       TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						                       IntData = new ObservableCollection<S.IItemData<int>>{
							                   new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Partner"].Value },
							                   new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
						                    }
					                    },
					                    new ItemDefinition{
						                    TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						                    IntData = new ObservableCollection<S.IItemData<int>>{new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Naziv"].Value },
					                        new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
						                    }
					                    },
					                    new ItemDefinition{
						                    TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						                    IntData = new ObservableCollection<S.IItemData<int>>{
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Premium"].Value },
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
						                    }
					                    },
					                    new ItemDefinition{
						                    TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						                    IntData = new ObservableCollection<S.IItemData<int>>{
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Cijena"].Value },
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
						                    }
					                    },
                                        new ItemDefinition{
								            TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								            IntData = new ObservableCollection<S.IItemData<int>>{
									            new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Datum dražbe"].Value },
									            new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
								            }
							            },
                                        new ItemDefinition{
                                            TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
                                            IntData = new ObservableCollection<S.IItemData<int>>{
                                                new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Slike"].Value },
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
                                            }
                                        },
						                new ItemDefinition{
							                TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
							                IntData = new ObservableCollection<S.IItemData<int>>{
								                new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Koordinate"].Value },
								                new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
							                }
						                },
					new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "INFORMACIJE O PREDMETU " }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
						},
						 Children= new ObservableCollection<S.IItem>{

							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Vrsta plovila"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
    						new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Marka"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Model"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Dužina"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Godina proizvodnje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Motor"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Radni obujam"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Snaga motora"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
								}
							},
						 },
					},
					new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "DODATNE INFORMACIJE" }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=9 },
						},
						 Children= new ObservableCollection<S.IItem>{
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Oprema vozila"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
                            new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Opis oglasa"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								}
							},
						}
					},
					new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "INFORMACIJE O PRODAJI" }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=10 },
						},
						 Children= new ObservableCollection<S.IItem>{
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj predmeta (stečaj/ovrha)"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Razgledavanje predmeta"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Adresa prodaje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Detalji prodaje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								}
							},
						 }
					},
				}

            });
            #endregion

            #region GOSPODARSKA PLOVILA
            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplate,
                StringData = new ObservableCollection<S.IItemData<string>>() { new ItemDefinitionDataString { TypeId = 1, Value = "Gospodarska plovila" } },
                Children = new ObservableCollection<S.IItem>{
					                       new ItemDefinition{
						                       TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						                       IntData = new ObservableCollection<S.IItemData<int>>{
							                   new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Partner"].Value },
							                   new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
						                    }
					                    },
					                    new ItemDefinition{
						                    TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						                    IntData = new ObservableCollection<S.IItemData<int>>{new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Naziv"].Value },
					                        new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
						                    }
					                    },
					                    new ItemDefinition{
						                    TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						                    IntData = new ObservableCollection<S.IItemData<int>>{
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Premium"].Value },
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
						                    }
					                    },
					                    new ItemDefinition{
						                    TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						                    IntData = new ObservableCollection<S.IItemData<int>>{
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Cijena"].Value },
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
						                    }
					                    },
                                        new ItemDefinition{
								            TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								            IntData = new ObservableCollection<S.IItemData<int>>{
									            new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Datum dražbe"].Value },
									            new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
								            }
							            },
                                        new ItemDefinition{
                                            TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
                                            IntData = new ObservableCollection<S.IItemData<int>>{
                                                new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Slike"].Value },
							                    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
                                            }
                                        },
						                new ItemDefinition{
							                TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
							                IntData = new ObservableCollection<S.IItemData<int>>{
								                new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Koordinate"].Value },
								                new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
							                }
						                },
					new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "INFORMACIJE O PREDMETU " }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
						},
						 Children= new ObservableCollection<S.IItem>{

							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Vrsta"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
    						new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Marka"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Model"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Dužina"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Godina proizvodnje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
								},
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Motor"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Radni obujam"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Snaga motora"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
								}
							}, 
						 },
					},
					new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "DODATNE INFORMACIJE" }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=9 },
						},
						 Children= new ObservableCollection<S.IItem>{
						
                            new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Opis oglasa"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
						}
					},
					new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "INFORMACIJE O PRODAJI" }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=10 },
						},
						 Children= new ObservableCollection<S.IItem>{
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj predmeta (stečaj/ovrha)"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Razgledavanje predmeta"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Adresa prodaje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Detalji prodaje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								}
							},
						 }
					},
				}

            });


            #endregion

            #region OSTALO

            context.ItemDefinition.Add(new ItemDefinition
            {
                TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplate,
                StringData = new ObservableCollection<S.IItemData<string>>() { new ItemDefinitionDataString { TypeId = 1, Value = "Ostalo 2" } },
                Children = new ObservableCollection<S.IItem>{
 					new ItemDefinition{
						    TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						    IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Partner"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
						}
					},
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Naziv"].Value },
					    new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
						}
					},
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Premium"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
						}
					},
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Cijena"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
						}
					},
                    new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Datum dražbe"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=5 },
						}
					},
                     new ItemDefinition{
                        TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
                        IntData = new ObservableCollection<S.IItemData<int>>{
                            new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Slike"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=6 },
                        }
                    },
					new ItemDefinition{
						TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Koordinate"].Value },
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=7 },
						}
					},
					new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "DODATNE INFORMACIJE" }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=8 },
						},
						 Children= new ObservableCollection<S.IItem>{
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Opis oglasa"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
						}
					},
                    	new ItemDefinition{
						 TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttributeGroup,
						 StringData = new ObservableCollection<S.IItemData<string>>{
							new ItemDefinitionDataString{ TypeId = 1, Value = "INFORMACIJE O PRODAJI" }
						 },
						IntData = new ObservableCollection<S.IItemData<int>>{
							new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=9 },
						},
						 Children= new ObservableCollection<S.IItem>{
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Broj predmeta (stečaj/ovrha)"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=1 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Razgledavanje predmeta"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=2 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Adresa prodaje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=3 },
								}
							},
							new ItemDefinition{
								TypeId = (int)ItemDefinitionTypeEnum.AttributeTemplateAttribute,
								IntData = new ObservableCollection<S.IItemData<int>>{
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateAttributeId,Value=atts["Detalji prodaje"].Value },
									new ItemDefinitionDataInt{ TypeId=(int)ItemDefinitionDataTypeEnum.AttributeTemplateOrder, Value=4 },
								}
							},
						 }
					},
				}
            });
            #endregion
            #endregion
            context.SaveChanges();
            #endregion
        }
    }
}
