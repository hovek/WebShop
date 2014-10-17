CREATE PROCEDURE spGetInconsistentItemNodes
AS 
SELECT DISTINCT
		id.*
FROM	dbo.Item id
LEFT JOIN dbo.Item i ON id.Node = i.Node
						AND id.Id != i.Id
LEFT JOIN dbo.Item parent ON id.ParentId = parent.Id
							 AND ( id.Node.IsDescendantOf(parent.Node) = 0
								   OR id.Node.GetLevel() - parent.Node.GetLevel() != 1 )
LEFT JOIN dbo.ItemDataInt iddi ON iddi.ItemId = id.Id
								  AND ( iddi.Node != id.Node
										OR iddi.ObjectNode != id.ObjectNode )
LEFT JOIN dbo.ItemDataDecimal iddd ON iddd.ItemId = id.Id
									  AND ( iddd.Node != id.Node
											OR iddd.ObjectNode != id.ObjectNode )
LEFT JOIN dbo.ItemDataDateTime idddt ON idddt.ItemId = id.Id
										AND ( idddt.Node != id.Node
											  OR idddt.ObjectNode != id.ObjectNode )
LEFT JOIN dbo.ItemDataString idds ON idds.ItemId = id.Id
									 AND ( idds.Node != id.Node
										   OR idds.ObjectNode != id.ObjectNode )
WHERE	id.Node.IsDescendantOf(id.ObjectNode) = 0
		OR ( id.ParentId IS NULL
			 AND id.Node.GetLevel() != 1 )
		OR parent.Id IS NOT NULL
		OR i.Id IS NOT NULL
		OR iddi.Id IS NOT NULL
		OR iddd.Id IS NOT NULL
		OR idddt.Id IS NOT NULL
		OR idds.Id IS NOT NULL


SELECT DISTINCT
		id.*
FROM	dbo.ItemDefinition id
LEFT JOIN dbo.ItemDefinition i ON id.Node = i.Node
								  AND id.Id != i.Id
LEFT JOIN dbo.ItemDefinition parent ON id.ParentId = parent.Id
									   AND ( id.Node.IsDescendantOf(parent.Node) = 0
											 OR id.Node.GetLevel() - parent.Node.GetLevel() != 1 )
LEFT JOIN dbo.ItemDefinitionDataInt iddi ON iddi.ItemDefinitionId = id.Id
											AND ( iddi.Node != id.Node
												  OR iddi.ObjectNode != id.ObjectNode )
LEFT JOIN dbo.ItemDefinitionDataDecimal iddd ON iddd.ItemDefinitionId = id.Id
												AND ( iddd.Node != id.Node
													  OR iddd.ObjectNode != id.ObjectNode )
LEFT JOIN dbo.ItemDefinitionDataDateTime idddt ON idddt.ItemDefinitionId = id.Id
												  AND ( idddt.Node != id.Node
														OR idddt.ObjectNode != id.ObjectNode )
LEFT JOIN dbo.ItemDefinitionDataString idds ON idds.ItemDefinitionId = id.Id
											   AND ( idds.Node != id.Node
													 OR idds.ObjectNode != id.ObjectNode )
WHERE	id.Node.IsDescendantOf(id.ObjectNode) = 0
		OR ( id.ParentId IS NULL
			 AND id.Node.GetLevel() != 1 )
		OR parent.Id IS NOT NULL
		OR i.Id IS NOT NULL
		OR iddi.Id IS NOT NULL
		OR iddd.Id IS NOT NULL
		OR idddt.Id IS NOT NULL
		OR idds.Id IS NOT NULL