CREATE PROCEDURE spGetClosestParentId
	@childId INT,
	@typeId VARCHAR(100) = NULL
AS 
DECLARE	@tblParentTypes TABLE ( id INT )
INSERT	INTO @tblParentTypes
		SELECT	item
		FROM	dbo.ftConvertDelimitedListOfIntIntoTable(@typeId, ',')

SELECT TOP 1
		p.Id
FROM	dbo.Item i
INNER JOIN dbo.Item p ON i.Id = @childId
						 AND i.Node.IsDescendantOf(p.Node) = 1
WHERE	@typeId IS NULL
		OR p.TypeID IN ( SELECT	id
						 FROM	@tblParentTypes )
ORDER BY p.Node DESC