CREATE PROCEDURE spGetProduct @groupId INT
AS 
DECLARE	@tblObjects TABLE ( Node HIERARCHYID )
INSERT	INTO @tblObjects
		SELECT	p.Node
		FROM	dbo.Item g
		INNER JOIN dbo.Item p ON g.Id = @groupId
								 AND p.TypeId = 3 --product
								 AND p.Node.GetAncestor(1) = g.Node

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

SELECT	ids.Id,
		ids.TypeId,
		ids.ItemId,
		ids.Value
FROM	dbo.ItemDataDecimal ids
INNER JOIN @tblObjects td ON ids.ObjectNode = td.Node

SELECT	ids.Id,
		ids.TypeId,
		ids.ItemId,
		ids.Value
FROM	dbo.ItemDataDateTime ids
INNER JOIN @tblObjects td ON ids.ObjectNode = td.Node