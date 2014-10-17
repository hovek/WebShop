CREATE PROCEDURE spGetGroups
	@getGroupsByItemParentId INT = NULL,
	@includeShowAttribute BIT = 0,
	@languageId INT = NULL
AS 
DECLARE	@tblObjects TABLE ( Node HIERARCHYID )
DECLARE	@showAttributeId INT
IF ( @includeShowAttribute = 1 ) 
	SELECT	@showAttributeId = a.ItemDefinitionId
	FROM	dbo.ItemDefinitionDataInt a
	WHERE	a.TypeId = 65 -- AttributeKey
			AND a.Value = 19 --Show

IF @getGroupsByItemParentId IS NOT NULL 
	BEGIN
		INSERT	INTO @tblObjects
				SELECT	g.Node
				FROM	dbo.Item i
				INNER JOIN Item g ON i.Id = @getGroupsByItemParentId
									 AND g.Node.IsDescendantOf(i.Node) = 1
									 AND g.TypeId = 2
				WHERE	NOT EXISTS ( SELECT TOP 1
											c.Id
									 FROM	dbo.Item c
									 INNER JOIN dbo.ItemDataInt a ON a.ObjectNode = c.Node
									 INNER JOIN dbo.ItemDataInt v ON v.ItemId = a.ItemId
									 WHERE	g.Node.IsDescendantOf(c.Node) = 1
											AND c.TypeId IN ( 1, 2 )
											AND a.TypeId = 51
											AND a.Value = @showAttributeId
											AND v.TypeId IN ( 0, @languageId )
											AND v.Value = 0 )

	END
ELSE 
	BEGIN
		INSERT	INTO @tblObjects
				SELECT	Node
				FROM	dbo.Item i
				WHERE	i.TypeId IN ( 1, 2 )
						AND NOT EXISTS ( SELECT TOP 1
												c.Id
										 FROM	dbo.Item c
										 INNER JOIN dbo.ItemDataInt a ON a.ObjectNode = c.Node
										 INNER JOIN dbo.ItemDataInt v ON v.ItemId = a.ItemId
										 WHERE	i.Node.IsDescendantOf(c.Node) = 1
												AND c.TypeId IN ( 1, 2 )
												AND a.TypeId = 51
												AND a.Value = @showAttributeId
												AND v.TypeId IN ( 0, @languageId )
												AND v.Value = 0 )
	END
    
SELECT	i.Id,
		i.TypeId,
		i.ParentId
FROM	@tblObjects td
INNER JOIN dbo.Item i ON i.ObjectNode = td.Node

SELECT	ids.Id,
		ids.TypeId,
		ids.ItemId,
		ids.Value
FROM	dbo.ItemDataString ids
INNER JOIN @tblObjects td ON ids.ObjectNode = td.Node

SELECT	ids.Id,
		ids.TypeId,
		ids.ItemId,
		ids.Value
FROM	dbo.ItemDataInt ids
INNER JOIN @tblObjects td ON ids.ObjectNode = td.Node