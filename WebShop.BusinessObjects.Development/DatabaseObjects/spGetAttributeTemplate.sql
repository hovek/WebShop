CREATE PROCEDURE spGetAttributeTemplate
	@templateID INT = NULL,
	@itemID INT = NULL,
	@allowGetFromNearestParent BIT = 1
AS 
DECLARE	@tblObjects TABLE ( Node HIERARCHYID )

IF @templateID IS NOT NULL 
	BEGIN
		INSERT	INTO @tblObjects
				SELECT	Node
				FROM	dbo.ItemDefinition i
				WHERE	i.TypeId = 4 --AttributeTemplate
						AND i.Id = @templateID 
	END 

IF @itemID IS NOT NULL 
	BEGIN
		--prvo dohvaćamo id template atributa na osnovu key-a
		DECLARE	@attributeTemplateId INT
		SELECT	@attributeTemplateId = iddi.ItemDefinitionId
		FROM	dbo.ItemDefinitionDataInt iddi
		WHERE	iddi.TypeId = 65 --AttributeKey
				AND iddi.Value = 5 --Template

		--dohvat noda od templatea od proslijeđenog itema
		DECLARE	@itemNode HIERARCHYID
		DECLARE	@templateNode HIERARCHYID      
		SELECT	@itemNode = i.Node,
				@templateNode = template.Node
		FROM	dbo.Item i
		LEFT JOIN dbo.ItemDataInt idi ON idi.ObjectNode = i.Node
										 AND idi.TypeId = 51--AttributeId
										 AND idi.Value = @attributeTemplateId
		LEFT JOIN dbo.ItemDataInt templateId ON templateId.Node = idi.Node
												AND templateId.TypeId = 0
		LEFT JOIN dbo.ItemDefinition template ON template.Id = templateId.Value
		WHERE	i.Id = @itemID
		
		--ako za proslijeđeni item ne postoji template onda dohvaća od najbližeg roditelja
		IF @templateNode IS NULL
			AND @allowGetFromNearestParent = 1 
			BEGIN      
				INSERT	INTO @tblObjects
						SELECT	MAX(template.Node)
						FROM	dbo.ItemDataInt idi
						INNER JOIN dbo.ItemDataInt templateId ON templateId.Node = idi.Node
																 AND @itemNode.IsDescendantOf(idi.ObjectNode) = 1
																 AND idi.TypeId = 51--AttributeId
																 AND idi.Value = @attributeTemplateId
																 AND templateId.TypeId = 0
						INNER JOIN dbo.ItemDefinition template ON template.Id = templateId.Value
			END   
		ELSE 
			BEGIN 
				INSERT	INTO @tblObjects
						SELECT	@templateNode
			END            
	END
ELSE 
	IF @templateID IS NULL 
		BEGIN 
			INSERT	INTO @tblObjects
					SELECT	Node
					FROM	dbo.ItemDefinition i
					WHERE	i.TypeId = 4
		END

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