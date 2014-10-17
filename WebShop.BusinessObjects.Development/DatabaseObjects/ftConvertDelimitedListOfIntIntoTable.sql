CREATE FUNCTION [dbo].[ftConvertDelimitedListOfIntIntoTable]
(
  @list VARCHAR(MAX),
  @delimiter CHAR(1)
)
RETURNS @table TABLE ( item INT NOT NULL )
AS 
BEGIN
	DECLARE	@pos INT,
		@nextpos INT,
		@valuelen INT,
		@value VARCHAR(20)

	SELECT	@pos = 0,
			@nextpos = 1

	WHILE @nextpos > 0 
		BEGIN
			SELECT	@nextpos = CHARINDEX(@delimiter, @list, @pos + 1)
			SELECT	@valuelen = CASE WHEN @nextpos > 0 THEN @nextpos
									 ELSE LEN(@list) + 1
								END - @pos - 1

			SET @value = SUBSTRING(@list, @pos + 1, @valuelen)
			IF LEN(@value) != 0 
				BEGIN 
					INSERT	@table
							( item )
					VALUES	( CONVERT(INT, @value) )
				END 
			SELECT	@pos = @nextpos
		END

	RETURN 
END