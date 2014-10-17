CREATE PROCEDURE spGetAttributeDefinition
AS 
DECLARE	@tblObjects TABLE ( Node HIERARCHYID )
INSERT	INTO @tblObjects
		SELECT	Node
		FROM	dbo.ItemDefinition i
		WHERE	i.TypeId = 1

SELECT	i.Id,
		i.TypeId,
		i.ParentId
FROM	@tblObjects td
INNER JOIN dbo.ItemDefinition i ON i.ObjectNode = td.Node

SELECT	ids.Id,
		ids.TypeId,
		ids.ItemDefinitionId,
		ids.Value
FROM	dbo.ItemDefinitionDataString ids
INNER JOIN @tblObjects td ON ids.ObjectNode = td.Node

SELECT	ids.Id,
		ids.TypeId,
		ids.ItemDefinitionId,
		ids.Value
FROM	dbo.ItemDefinitionDataInt ids
INNER JOIN @tblObjects td ON ids.ObjectNode = td.Node