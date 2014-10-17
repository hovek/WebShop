CREATE PROCEDURE spGetItem
	@itemId INT,
	@attributeIds VARCHAR(MAX) = NULL
AS 
DECLARE	@tblAttributes TABLE ( id INT )
INSERT	INTO @tblAttributes
		SELECT	item
		FROM	dbo.ftConvertDelimitedListOfIntIntoTable(@attributeIds, ',')

DECLARE	@tblObjects TABLE ( Node HIERARCHYID )
INSERT	INTO @tblObjects
		SELECT	a.Node
		FROM	dbo.Item i
		INNER JOIN dbo.Item a ON a.ObjectNode = i.ObjectNode
		WHERE	i.Id = @itemId
				AND ( a.TypeId != 4 --Attribute
					  OR @attributeIds IS NULL
					  OR EXISTS ( SELECT	id
								  FROM		dbo.ItemDataInt
								  WHERE		TypeId = 51 -- AttributeId
											AND ItemId = a.Id
											AND Value IN ( SELECT	id
														   FROM		@tblAttributes ) ) ) 

SELECT	i.Id,
		i.TypeId,
		i.ParentId
FROM	@tblObjects td
INNER JOIN dbo.Item i ON i.Node = td.Node

SELECT	ids.Id,
		ids.TypeId,
		ids.ItemId,
		ids.Value
FROM	dbo.ItemDataString ids
INNER JOIN @tblObjects td ON ids.Node = td.Node

SELECT	ids.Id,
		ids.TypeId,
		ids.ItemId,
		ids.Value
FROM	dbo.ItemDataInt ids
INNER JOIN @tblObjects td ON ids.Node = td.Node

SELECT	ids.Id,
		ids.TypeId,
		ids.ItemId,
		ids.Value
FROM	dbo.ItemDataDecimal ids
INNER JOIN @tblObjects td ON ids.Node = td.Node

SELECT	ids.Id,
		ids.TypeId,
		ids.ItemId,
		ids.Value
FROM	dbo.ItemDataDateTime ids
INNER JOIN @tblObjects td ON ids.Node = td.Node