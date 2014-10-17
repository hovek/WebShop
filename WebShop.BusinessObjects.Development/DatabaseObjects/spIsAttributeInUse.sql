CREATE PROCEDURE spIsAttributeInUse
	@AttributeDefinitionId INT
AS 
DECLARE	@rez BIT

IF ( EXISTS ( SELECT TOP 1
						attribute.Id
			  FROM		dbo.ItemDataInt attribute
			  WHERE		attribute.TypeId = 51--AttributeId
						AND attribute.Value = @AttributeDefinitionId ) ) 
	SET @rez = 1
ELSE 
	IF ( EXISTS ( SELECT TOP 1
							i.Id
				  FROM		dbo.ItemDefinitionDataInt i
				  WHERE		i.TypeId = 57--AttributeTemplateAttributeId
							AND i.Value = @AttributeDefinitionId ) ) 
		SET @rez = 1
	ELSE 
		SET @rez = 0

SELECT	@rez 