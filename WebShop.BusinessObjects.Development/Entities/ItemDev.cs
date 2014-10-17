using System.Collections.Generic;
using System.Linq;
using Syrilium.Modules.BusinessObjects.Item;
using Syrilium.Modules.BusinessObjects;
using itm = Syrilium.Modules.BusinessObjects.Item;
using S = Syrilium.Interfaces.BusinessObjectsInterface.Item;
using WebShop.BusinessObjectsInterface.Item;
using Syrilium.CommonInterface;

namespace WebShop.BusinessObjects.Development.Entities
{
    public class ItemDev
    {
        public static void ExecuteDDL(WebShopDb context)
        {
            #region Item
            context.Database.ExecuteSqlCommand(
                @"SET ANSI_NULLS ON

				ALTER TABLE Item
				ADD Node hierarchyid not null,
					ObjectNode hierarchyid not null

				CREATE UNIQUE NONCLUSTERED INDEX [IX_Node] ON [dbo].[Item] 
				(
					[Node] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

				CREATE NONCLUSTERED INDEX [IX_ObjectNode] ON [dbo].[Item] 
				(
					[ObjectNode] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
				
				CREATE NONCLUSTERED INDEX [IX_TypeId] ON [dbo].[Item] 
				(
					[TypeId] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]");

            string objectIds = string.Format("{0}, {1}, {2}", (int)ItemTypeEnum.Product, (int)ItemTypeEnum.Group, (int)ItemTypeEnum.Department);

            context.Database.ExecuteSqlCommand(
                @"CREATE TRIGGER [dbo].[ItemInsteadOfInsert] ON [dbo].[Item]
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
							LEFT JOIN dbo.Item i ON ins.ParentId = i.ParentId
						END
					ELSE 
						BEGIN
							SELECT	@MinNode = MIN(i.Node),
									@MaxNode = MAX(i.Node)
							FROM	dbo.Item i
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

					INSERT	INTO dbo.Item
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
									  LEFT JOIN Item p ON p.Id = i.ParentId
									) i
							OUTER APPLY ( SELECT TOP 1
													o.Node
										  FROM		dbo.Item o
										  WHERE		isnull(i.TypeId,0) NOT IN ( " + objectIds + @" ) AND o.TypeId IN ( " + objectIds + @" ) AND i.Node.IsDescendantOf(o.Node) = 1
										  ORDER BY	o.Node DESC
										) o

					SELECT	SCOPE_IDENTITY() AS Id
				END");
            #endregion
            #region ItemDataString
            context.Database.ExecuteSqlCommand(
                @"ALTER TABLE ItemDataString
				ADD Node hierarchyid not null,
					ObjectNode hierarchyid not null

				CREATE NONCLUSTERED INDEX [IX_Node] ON [dbo].[ItemDataString] 
				(
					[Node] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

				CREATE NONCLUSTERED INDEX [IX_ObjectNode] ON [dbo].[ItemDataString] 
				(
					[ObjectNode] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
				
				CREATE NONCLUSTERED INDEX [IX_TypeId] ON [dbo].[ItemDataString] 
				(
					[TypeId] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

				CREATE NONCLUSTERED INDEX [IX_Value] ON [dbo].[ItemDataString] 
				(
					[Value] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]");

            context.Database.ExecuteSqlCommand(
                @"CREATE TRIGGER ItemDataStringInsteadOfInsert ON dbo.ItemDataString
					INSTEAD OF INSERT
				AS
				BEGIN
					INSERT	INTO dbo.ItemDataString
							( TypeId,
							  ItemId,
							  Value,
								Node,
							  ObjectNode )
							SELECT	i.TypeId,
									i.ItemId,
									i.Value,
									it.Node,
									it.ObjectNode
							FROM	INSERTED i
							LEFT JOIN dbo.Item it ON i.ItemId = it.Id

					SELECT	SCOPE_IDENTITY() AS Id
				END");

			context.Database.ExecuteSqlCommand(
				@"CREATE TRIGGER ItemDataStringAfterUpdate ON [dbo].[ItemDataString]
					AFTER UPDATE
				AS
				UPDATE	id
				SET		Node = i.Node,
						ObjectNode = i.ObjectNode
				FROM	INSERTED ins
				INNER JOIN DELETED del ON ins.Id = del.Id
										  AND ins.ItemId != del.ItemId
				INNER JOIN ItemDataString id ON ins.Id = id.Id
				INNER JOIN dbo.Item i ON id.ItemId = i.Id");
			#endregion
            #region ItemDataInt
            context.Database.ExecuteSqlCommand(
                @"ALTER TABLE ItemDataInt
				ADD Node hierarchyid not null,
					ObjectNode hierarchyid not null

				CREATE NONCLUSTERED INDEX [IX_Node] ON [dbo].[ItemDataInt] 
				(
					[Node] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

				CREATE NONCLUSTERED INDEX [IX_ObjectNode] ON [dbo].[ItemDataInt] 
				(
					[ObjectNode] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
				
				CREATE NONCLUSTERED INDEX [IX_TypeId] ON [dbo].[ItemDataInt] 
				(
					[TypeId] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

				CREATE NONCLUSTERED INDEX [IX_Value] ON [dbo].[ItemDataInt] 
				(
					[Value] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]");

            context.Database.ExecuteSqlCommand(
                @"CREATE TRIGGER ItemDataIntInsteadOfInsert ON dbo.ItemDataInt
					INSTEAD OF INSERT
				AS
				BEGIN
					INSERT	INTO dbo.ItemDataInt
							( TypeId,
							  ItemId,
							  Value,
								Node,
							  ObjectNode )
							SELECT	i.TypeId,
									i.ItemId,
									i.Value,
									it.Node,
									it.ObjectNode
							FROM	INSERTED i
							LEFT JOIN dbo.Item it ON i.ItemId = it.Id

					SELECT	SCOPE_IDENTITY() AS Id
				END");

			context.Database.ExecuteSqlCommand(
				@"CREATE TRIGGER ItemDataIntAfterUpdate ON [dbo].[ItemDataInt]
					AFTER UPDATE
				AS
				UPDATE	id
				SET		Node = i.Node,
						ObjectNode = i.ObjectNode
				FROM	INSERTED ins
				INNER JOIN DELETED del ON ins.Id = del.Id
										  AND ins.ItemId != del.ItemId
				INNER JOIN ItemDataInt id ON ins.Id = id.Id
				INNER JOIN dbo.Item i ON id.ItemId = i.Id");
			#endregion
            #region ItemDataDecimal
            context.Database.ExecuteSqlCommand(
                @"ALTER TABLE ItemDataDecimal
				ADD Node hierarchyid not null,
					ObjectNode hierarchyid not null

				CREATE NONCLUSTERED INDEX [IX_Node] ON [dbo].[ItemDataDecimal] 
				(
					[Node] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

				CREATE NONCLUSTERED INDEX [IX_ObjectNode] ON [dbo].[ItemDataDecimal] 
				(
					[ObjectNode] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
				
				CREATE NONCLUSTERED INDEX [IX_TypeId] ON [dbo].[ItemDataDecimal] 
				(
					[TypeId] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

				CREATE NONCLUSTERED INDEX [IX_Value] ON [dbo].[ItemDataDecimal] 
				(
					[Value] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]");

            context.Database.ExecuteSqlCommand(
                @"CREATE TRIGGER ItemDataDecimalInsteadOfInsert ON dbo.ItemDataDecimal
					INSTEAD OF INSERT
				AS
				BEGIN
					INSERT	INTO dbo.ItemDataDecimal
							( TypeId,
							  ItemId,
							  Value,
								Node,
							  ObjectNode )
							SELECT	i.TypeId,
									i.ItemId,
									i.Value,
									it.Node,
									it.ObjectNode
							FROM	INSERTED i
							LEFT JOIN dbo.Item it ON i.ItemId = it.Id

					SELECT	SCOPE_IDENTITY() AS Id
				END");

			context.Database.ExecuteSqlCommand(
				@"CREATE TRIGGER ItemDataDecimalAfterUpdate ON [dbo].[ItemDataDecimal]
					AFTER UPDATE
				AS
				UPDATE	id
				SET		Node = i.Node,
						ObjectNode = i.ObjectNode
				FROM	INSERTED ins
				INNER JOIN DELETED del ON ins.Id = del.Id
										  AND ins.ItemId != del.ItemId
				INNER JOIN ItemDataDecimal id ON ins.Id = id.Id
				INNER JOIN dbo.Item i ON id.ItemId = i.Id");
			#endregion
            #region ItemDataDateTime
            context.Database.ExecuteSqlCommand(
                @"ALTER TABLE ItemDataDateTime
				ADD Node hierarchyid not null,
					ObjectNode hierarchyid not null

				CREATE NONCLUSTERED INDEX [IX_Node] ON [dbo].[ItemDataDateTime] 
				(
					[Node] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

				CREATE NONCLUSTERED INDEX [IX_ObjectNode] ON [dbo].[ItemDataDateTime] 
				(
					[ObjectNode] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
				
				CREATE NONCLUSTERED INDEX [IX_TypeId] ON [dbo].[ItemDataDateTime] 
				(
					[TypeId] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

				CREATE NONCLUSTERED INDEX [IX_Value] ON [dbo].[ItemDataDateTime] 
				(
					[Value] ASC
				)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]");

            context.Database.ExecuteSqlCommand(
                @"CREATE TRIGGER ItemDataDateTimeInsteadOfInsert ON dbo.ItemDataDateTime
					INSTEAD OF INSERT
				AS
				BEGIN
					INSERT	INTO dbo.ItemDataDateTime
							( TypeId,
							  ItemId,
							  Value,
								Node,
							  ObjectNode )
							SELECT	i.TypeId,
									i.ItemId,
									i.Value,
									it.Node,
									it.ObjectNode
							FROM	INSERTED i
							LEFT JOIN dbo.Item it ON i.ItemId = it.Id

					SELECT	SCOPE_IDENTITY() AS Id
				END");

			context.Database.ExecuteSqlCommand(
				@"CREATE TRIGGER ItemDataDateTimeAfterUpdate ON [dbo].[ItemDataDateTime]
					AFTER UPDATE
				AS
				UPDATE	id
				SET		Node = i.Node,
						ObjectNode = i.ObjectNode
				FROM	INSERTED ins
				INNER JOIN DELETED del ON ins.Id = del.Id
										  AND ins.ItemId != del.ItemId
				INNER JOIN ItemDataDateTime id ON ins.Id = id.Id
				INNER JOIN dbo.Item i ON id.ItemId = i.Id");
			#endregion
			#region ItemExtent
			context.Database.ExecuteSqlCommand(
				@"CREATE TRIGGER [dbo].[ItemInsteadOfUpdate] ON [dbo].[Item]
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
					FROM	dbo.Item id
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
											FROM	dbo.Item i
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
											FROM	dbo.Item i
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
									FROM	dbo.Item i
									LEFT JOIN Item p ON p.Id = @ParentId
									--dohvaća Node
									CROSS APPLY ( SELECT	CASE WHEN p.Id IS NULL THEN HIERARCHYID::GetRoot().GetDescendant(@LeftChild, @RightChild)
																 ELSE p.Node.GetDescendant(@LeftChild, @RightChild)
															END AS Node
												) n
									--dohvaća ObjectNode
									OUTER APPLY ( SELECT TOP 1
															o.Node
												  FROM		dbo.Item o
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
									FROM	dbo.ItemDataInt id
									INNER JOIN Item i ON id.ItemId = i.Id
									WHERE	i.Id = @Id

									UPDATE	id
									SET		Node = i.Node,
											ObjectNode = i.ObjectNode
									FROM	dbo.ItemDataDecimal id
									INNER JOIN Item i ON id.ItemId = i.Id
									WHERE	i.Id = @Id

									UPDATE	id
									SET		Node = i.Node,
											ObjectNode = i.ObjectNode
									FROM	dbo.ItemDataString id
									INNER JOIN Item i ON id.ItemId = i.Id
									WHERE	i.Id = @Id

									UPDATE	id
									SET		Node = i.Node,
											ObjectNode = i.ObjectNode
									FROM	dbo.ItemDataDateTime id
									INNER JOIN Item i ON id.ItemId = i.Id
									WHERE	i.Id = @Id

									--dohvaća sve childove jedan nivo niže za trenutni item i inserta ih u privremenu tablicu te ponovno prolazi kroz petlju za svaki od njih
									INSERT	INTO @tblTempRecordsForHierarchyUpdate
											SELECT	id.Node.GetLevel(),
													id.Id,
													@Id
											FROM	dbo.Item id
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
			@"CREATE TRIGGER [dbo].[ItemInsteadOfDelete] ON [dbo].[Item]
					INSTEAD OF DELETE
				AS
				BEGIN
					SET ANSI_WARNINGS OFF
					SET NOCOUNT ON 

					DELETE	i
					FROM	DELETED d
					INNER JOIN dbo.Item i ON i.Node.IsDescendantOf(d.Node) = 1
											 OR d.Id = i.Id

					DELETE	i
					FROM	DELETED d
					INNER JOIN dbo.ItemDataDateTime i ON i.Node.IsDescendantOf(d.Node) = 1

					DELETE	i
					FROM	DELETED d
					INNER JOIN dbo.ItemDataDecimal i ON i.Node.IsDescendantOf(d.Node) = 1

					DELETE	i
					FROM	DELETED d
					INNER JOIN dbo.ItemDataInt i ON i.Node.IsDescendantOf(d.Node) = 1

					DELETE	i
					FROM	DELETED d
					INNER JOIN dbo.ItemDataString i ON i.Node.IsDescendantOf(d.Node) = 1
				END");
			#endregion
		}

        public static void FillData(WebShopDb context)
        {
            var atts = (from ids in context.ItemDefinitionDataString
                        join i in context.ItemDefinition on ids.ItemDefinitionId equals i.Id
                        where i.TypeId == (int)ItemDefinitionTypeEnum.AttributeDefinition
                                && ids.TypeId == 1
                        select new { Key = ids.Value, Value = i.Id }).ToDictionary(d => d.Key);

            var templates = (from ids in context.ItemDefinitionDataString
                             join i in context.ItemDefinition on ids.ItemDefinitionId equals i.Id
                             where i.TypeId == (int)ItemDefinitionTypeEnum.AttributeTemplate
                                     && ids.TypeId == 1
                             select new { Key = ids.Value, Value = i.Id }).ToDictionary(d => d.Key);

            itm.Item grpStanovi =
                    new itm.Item
                    {
                        TypeId = (int)ItemTypeEnum.Group,
                        Children = new ObservableCollection<S.IItem> { 
							new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ new ItemDataInt{ TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Naziv"].Value }},
								StringData = new ObservableCollection<S.IItemData<string>> { 
									new ItemDataString { TypeId = 1, Value = "stanovi" },
									new ItemDataString { TypeId = 2, Value = "housing" }
								}
							},
							new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ 
									new ItemDataInt{ TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Predložak"].Value },
									new ItemDataInt{ Value = templates["Stan"].Value }
								},
							},
						}
                    };

            #region Departments
            context.Item.Add(new itm.Item
            {
                TypeId = (int)ItemTypeEnum.Department,
                Children = new ObservableCollection<S.IItem> { 
					new itm.Item { 
						TypeId = (int)ItemTypeEnum.Attribute,
						IntData = new ObservableCollection<S.IItemData<int>>{ new ItemDataInt{ TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Naziv"].Value }},
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDataString { TypeId = 1, Value = "NEKRETNINE" },
							new ItemDataString { TypeId = 2, Value = "REAL ESTATE" }
						}
					},
					new itm.Item { 
						TypeId = (int)ItemTypeEnum.Attribute,
						IntData = new ObservableCollection<S.IItemData<int>>{ new ItemDataInt{ TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Sličica"].Value }},
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDataString { Value = ""/*/Resources/Images/HomeIconSecond.png*/ },
						}
					},
				    new itm.Item { 
				        TypeId = (int)ItemTypeEnum.Attribute,
				        IntData = new ObservableCollection<S.IItemData<int>>{ 
					        new ItemDataInt{ TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Predložak"].Value },
				        },
			        },
				#region Groups
					new itm.Item
					{
						TypeId = (int)ItemTypeEnum.Group,
						Children = new ObservableCollection<S.IItem> { 
							new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ new ItemDataInt{TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Naziv"].Value }},
								StringData = new ObservableCollection<S.IItemData<string>> { 
									new ItemDataString { TypeId = 1, Value = "apartmani" },
									new ItemDataString { TypeId = 2, Value = "apartments" }
								}
							},
							new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ 
									new ItemDataInt{ TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Predložak"].Value },
									new ItemDataInt{ Value = templates["Apartmani"].Value }
								},
							},
						}
					},
					new itm.Item
					{
						TypeId = (int)ItemTypeEnum.Group,
						Children = new ObservableCollection<S.IItem> { 
							new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ new ItemDataInt{TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Naziv"].Value }},
								StringData = new ObservableCollection<S.IItemData<string>> { 
									new ItemDataString { TypeId = 1, Value = "kuće" },
									new ItemDataString { TypeId = 2, Value = "houses" }
								}
							},
							new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ 
									new ItemDataInt{ TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Predložak"].Value },
									new ItemDataInt{ Value = templates["Kuće"].Value }
								},
							},
						}
					},
					grpStanovi,
                    	new itm.Item
					{
						TypeId = (int)ItemTypeEnum.Group,
						Children = new ObservableCollection<S.IItem> { 
							new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ new ItemDataInt{TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Naziv"].Value }},
								StringData = new ObservableCollection<S.IItemData<string>> { 
									new ItemDataString { TypeId = 1, Value = "Poslovni prostori i objekti" },
									new ItemDataString { TypeId = 2, Value = "Business premises and facilities" }
								}
							},
							new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ 
									new ItemDataInt{ TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Predložak"].Value },
									new ItemDataInt{ Value = templates["Poslovni prostori i objekti"].Value }
								},
							},
						}
					},
					new itm.Item
					{
						TypeId = (int)ItemTypeEnum.Group,
						Children = new ObservableCollection<S.IItem> { 
							new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ new ItemDataInt{TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Naziv"].Value }},
								StringData = new ObservableCollection<S.IItemData<string>> { 
									new ItemDataString { TypeId = 1, Value = "građevinska zemljišta" },
									new ItemDataString { TypeId = 2, Value = "plots" }
								}
							},
                            	new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ 
									new ItemDataInt{ TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Predložak"].Value },
									new ItemDataInt{ Value = templates["Građevinska zemljišta"].Value }
								},
							},
						}
					},
					new itm.Item
					{
						TypeId = (int)ItemTypeEnum.Group,
						Children = new ObservableCollection<S.IItem> { 
							new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ new ItemDataInt{TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Naziv"].Value }},
								StringData = new ObservableCollection<S.IItemData<string>> { 
									new ItemDataString { TypeId = 1, Value = "poljoprivredna zemljišta" },
									new ItemDataString { TypeId = 2, Value = "agricultural land" }
								}
							},
                            	new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ 
									new ItemDataInt{ TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Predložak"].Value },
									new ItemDataInt{ Value = templates["Poljoprivredna zemljišta"].Value }
								},
							},
						}
					},
					new itm.Item
					{
						TypeId = (int)ItemTypeEnum.Group,
						Children = new ObservableCollection<S.IItem> { 
							new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ new ItemDataInt{TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Naziv"].Value }},
								StringData = new ObservableCollection<S.IItemData<string>> { 
									new ItemDataString { TypeId = 1, Value = "Ostalo" },
									new ItemDataString { TypeId = 2, Value = "Other" }
								}
							},
                            	new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ 
									new ItemDataInt{ TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Predložak"].Value },
									new ItemDataInt{ Value = templates["Ostalo"].Value }
								},
							},
						}
					}
					#endregion
				}
            });

            context.Item.Add(new itm.Item
            {
                TypeId = (int)ItemTypeEnum.Department,
                Children = new ObservableCollection<S.IItem> { 
					new itm.Item { 
						TypeId = (int)ItemTypeEnum.Attribute,
						IntData = new ObservableCollection<S.IItemData<int>>{ new ItemDataInt{TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Naziv"].Value }},
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDataString { TypeId = 1, Value = "VOZILA" },
							new ItemDataString { TypeId = 2, Value = "VEHICLES" }
						}
					},
					new itm.Item { 
						TypeId = (int)ItemTypeEnum.Attribute,
						IntData = new ObservableCollection<S.IItemData<int>>{ new ItemDataInt{ TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Sličica"].Value }},
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDataString { Value = ""/*/Resources/Images/CarIconSecond.png*/ },
						}
					},
				    new itm.Item { 
				        TypeId = (int)ItemTypeEnum.Attribute,
				        IntData = new ObservableCollection<S.IItemData<int>>{ 
					        new ItemDataInt{ TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Predložak"].Value },
				        },
			        },
					#region Groups
					new itm.Item
					{
						TypeId = (int)ItemTypeEnum.Group,
						Children = new ObservableCollection<S.IItem> { 
							new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ new ItemDataInt{TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Naziv"].Value }},
								StringData = new ObservableCollection<S.IItemData<string>> { 
									new ItemDataString { TypeId = 1, Value = "Osobna vozila" },
									new ItemDataString { TypeId = 2, Value = "Personal vehicles" }
								}
							},
                            new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ 
									new ItemDataInt{ TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Predložak"].Value },
									new ItemDataInt{ Value = templates["Osobna vozila"].Value }
								},
							},
						}
					},
					new itm.Item
					{
						TypeId = (int)ItemTypeEnum.Group,
						Children = new ObservableCollection<S.IItem> { 
							new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ new ItemDataInt{TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Naziv"].Value }},
								StringData = new ObservableCollection<S.IItemData<string>> { 
									new ItemDataString { TypeId = 1, Value = "Gospodarska vozila" },
									new ItemDataString { TypeId = 2, Value = "Commercial vehicles" }
								}
							},
                             new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ 
									new ItemDataInt{ TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Predložak"].Value },
									new ItemDataInt{ Value = templates["Gospodarska vozila"].Value }
								},
							},
						}
					},
                    new itm.Item
					{
						TypeId = (int)ItemTypeEnum.Group,
						Children = new ObservableCollection<S.IItem> { 
							new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ new ItemDataInt{TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Naziv"].Value }},
								StringData = new ObservableCollection<S.IItemData<string>> { 
									new ItemDataString { TypeId = 1, Value = "Motori" },
									new ItemDataString { TypeId = 2, Value = "Motor vehicles" }
								}
							},
                             new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ 
									new ItemDataInt{ TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Predložak"].Value },
									new ItemDataInt{ Value = templates["Motori"].Value }
								},
							},
						}
					},
                    new itm.Item
					{
						TypeId = (int)ItemTypeEnum.Group,
						Children = new ObservableCollection<S.IItem> { 
							new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ new ItemDataInt{TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Naziv"].Value }},
								StringData = new ObservableCollection<S.IItemData<string>> { 
									new ItemDataString { TypeId = 1, Value = "Ostalo" },
									new ItemDataString { TypeId = 2, Value = "Other" }
								}
							},
                            	new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ 
									new ItemDataInt{ TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Predložak"].Value },
									new ItemDataInt{ Value = templates["Ostalo 1"].Value }
								},
							},
                    }
					}
                    #endregion
				}
            });

            context.Item.Add(new itm.Item
            {
                TypeId = (int)ItemTypeEnum.Department,
                Children = new ObservableCollection<S.IItem> { 
					new itm.Item { 
						TypeId = (int)ItemTypeEnum.Attribute,
						IntData = new ObservableCollection<S.IItemData<int>>{ new ItemDataInt{TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Naziv"].Value }},
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDataString { TypeId = 1, Value = "NAUTIKA" },
							new ItemDataString { TypeId = 2, Value = "NAUTICS" }
						}
					},
					new itm.Item { 
						TypeId = (int)ItemTypeEnum.Attribute,
						IntData = new ObservableCollection<S.IItemData<int>>{ new ItemDataInt{ TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Sličica"].Value }},
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDataString { Value = ""/*/Resources/Images/ShipIconSecond.png*/ },
						}
					},
				    new itm.Item { 
				        TypeId = (int)ItemTypeEnum.Attribute,
				        IntData = new ObservableCollection<S.IItemData<int>>{ 
					        new ItemDataInt{ TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Predložak"].Value },
				        },
			        },
					#region Groups
					new itm.Item
					{
						TypeId = (int)ItemTypeEnum.Group,
						Children = new ObservableCollection<S.IItem> { 
							new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ new ItemDataInt{TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Naziv"].Value }},
								StringData = new ObservableCollection<S.IItemData<string>> { 
									new ItemDataString { TypeId = 1, Value = "Osobna plovila" },
									new ItemDataString { TypeId = 2, Value = "Personal vessels" }
								}
							},
                            new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ 
									new ItemDataInt{ TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Predložak"].Value },
									new ItemDataInt{ Value = templates["Osobna plovila"].Value }
								},
							},
						}
					},
					new itm.Item
					{
						TypeId = (int)ItemTypeEnum.Group,
						Children = new ObservableCollection<S.IItem> { 
							new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ new ItemDataInt{TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Naziv"].Value }},
								StringData = new ObservableCollection<S.IItemData<string>> { 
									new ItemDataString { TypeId = 1, Value = "Gospodarska plovila" },
									new ItemDataString { TypeId = 2, Value = "Commercial vessels" }
								}
							},
                            new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ 
									new ItemDataInt{ TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Predložak"].Value },
									new ItemDataInt{ Value = templates["Gospodarska plovila"].Value }
								},
							},
						}
					},
                    new itm.Item
					{
						TypeId = (int)ItemTypeEnum.Group,
						Children = new ObservableCollection<S.IItem> { 
							new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ new ItemDataInt{TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Naziv"].Value }},
								StringData = new ObservableCollection<S.IItemData<string>> { 
									new ItemDataString { TypeId = 1, Value = "Ostalo" },
									new ItemDataString { TypeId = 2, Value = "Other" }
								}
							},
                            new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ 
									new ItemDataInt{ TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Predložak"].Value },
									new ItemDataInt{ Value = templates["Ostalo 2"].Value }
								},
							},
						}
					}
					#endregion
				}
            });

            context.Item.Add(new itm.Item
            {
                TypeId = (int)ItemTypeEnum.Department,
                Children = new ObservableCollection<S.IItem> { 
					new itm.Item { 
						TypeId = (int)ItemTypeEnum.Attribute,
						IntData = new ObservableCollection<S.IItemData<int>>{ new ItemDataInt{TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Naziv"].Value }},
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDataString { TypeId = 1, Value = "OSTALO" },
							new ItemDataString { TypeId = 2, Value = "OTHER" }
						}
					},
					new itm.Item { 
						TypeId = (int)ItemTypeEnum.Attribute,
						IntData = new ObservableCollection<S.IItemData<int>>{ new ItemDataInt{ TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Sličica"].Value }},
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDataString { Value = ""/*/Resources/Images/OtherIconSecond.png*/ },
						}
					},
				    new itm.Item { 
				        TypeId = (int)ItemTypeEnum.Attribute,
				        IntData = new ObservableCollection<S.IItemData<int>>{ 
					        new ItemDataInt{ TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Predložak"].Value },
				        },
			        },
					#region Groups
					new itm.Item
					{
						TypeId = (int)ItemTypeEnum.Group,
						Children = new ObservableCollection<S.IItem> { 
							new itm.Item { 
								TypeId = (int)ItemTypeEnum.Attribute,
								IntData = new ObservableCollection<S.IItemData<int>>{ new ItemDataInt{TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Naziv"].Value }},
								StringData = new ObservableCollection<S.IItemData<string>> { 
									new ItemDataString { TypeId = 1, Value = "strojevi" },
									new ItemDataString { TypeId = 2, Value = "machinery" }
								}
							},
							new itm.Item { 
							    TypeId = (int)ItemTypeEnum.Attribute,
							    IntData = new ObservableCollection<S.IItemData<int>>{ 
								    new ItemDataInt{ TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Predložak"].Value }
							    },
						    },
					    }
					},
					#endregion
				}
            });
            #endregion

            var vrstaList = (from i in context.ItemDefinition
                             join d in context.ItemDefinitionDataString on i.Id equals d.ItemDefinitionId
                             where i.TypeId == (int)ItemDefinitionTypeEnum.PredefinedList && d.Value == "Vrsta lista"
                             select i).First();

            var partnerId = context.Partner.First().Id;
            var countyId = context.County.First().Id;
            var districtCityId = context.DistrictCity.First().Id;

            grpStanovi.Children.Add(new itm.Item
            {
                TypeId = (int)ItemTypeEnum.Product,
                Children = new ObservableCollection<S.IItem> { 
					new itm.Item { 
						TypeId = (int)ItemTypeEnum.Attribute,
						IntData = new ObservableCollection<S.IItemData<int>>{ 
							new ItemDataInt{TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Premium"].Value },
							new ItemDataInt{ Value = 1 }
						}
					},
					new itm.Item { 
						TypeId = (int)ItemTypeEnum.Attribute,
						IntData = new ObservableCollection<S.IItemData<int>>{ 
							new ItemDataInt{TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Partner"].Value },
							new ItemDataInt{ Value = partnerId }
						}
					},
					new itm.Item { 
						TypeId = (int)ItemTypeEnum.Attribute,
						IntData = new ObservableCollection<S.IItemData<int>>{ new ItemDataInt{TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Naziv"].Value }},
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDataString { TypeId = 1, Value = "Stan | Zagreb - Markuševec" },
							new ItemDataString { TypeId = 2, Value = "Apartment | Zagreb - Markuševec" }
						}
					},
					new itm.Item { 
						TypeId = (int)ItemTypeEnum.Attribute,
						IntData = new ObservableCollection<S.IItemData<int>>{ new ItemDataInt{TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Slike"].Value }},
						StringData = new ObservableCollection<S.IItemData<string>> { 
							new ItemDataString { Value = @"ErsteLogo.png" },
							new ItemDataString { Value = @"HypoLogo.png" },
							new ItemDataString { Value = @"PBZLogo.png" }
						}
					},
					new itm.Item{
						TypeId = (int)ItemTypeEnum.Attribute,
						IntData = new ObservableCollection<S.IItemData<int>>{ 
							new ItemDataInt{TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Županija"].Value },
							new ItemDataInt { Value = countyId }
						},
					},
					new itm.Item{
						TypeId = (int)ItemTypeEnum.Attribute,
						IntData = new ObservableCollection<S.IItemData<int>>{ 
							new ItemDataInt{TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Općina/Grad"].Value },
							new ItemDataInt { Value = districtCityId }
						},
					},
					new itm.Item { 
						TypeId = (int)ItemTypeEnum.Attribute,
						IntData = new ObservableCollection<S.IItemData<int>>{ 
							new ItemDataInt{TypeId = (int)ItemDataTypeEnum.AttributeId, Value = atts["Cijena"].Value }
						},
						DecimalData = new ObservableCollection<S.IItemData<decimal>>{ 
							new ItemDataDecimal{ Value = 150000m }
						}
					},
			}
            });

            context.SaveChanges();
        }
    }
}
