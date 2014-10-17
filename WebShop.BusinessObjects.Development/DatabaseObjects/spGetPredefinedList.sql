CREATE PROCEDURE spGetPredefinedList @listId INT
AS 
DECLARE	@tblObjects TABLE ( Node HIERARCHYID )
INSERT	INTO @tblObjects
		SELECT	i.Node
		FROM	dbo.ItemDefinition i
		WHERE	i.Id = @listId

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

SELECT	ids.Id,
		ids.TypeId,
		ids.ItemDefinitionId,
		ids.Value
FROM	dbo.ItemDefinitionDataDecimal ids
INNER JOIN @tblObjects td ON ids.ObjectNode = td.Node

SELECT	ids.Id,
		ids.TypeId,
		ids.ItemDefinitionId,
		ids.Value
FROM	dbo.ItemDefinitionDataDateTime ids
INNER JOIN @tblObjects td ON ids.ObjectNode = td.Node