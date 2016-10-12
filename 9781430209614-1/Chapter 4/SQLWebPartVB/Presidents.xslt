<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<table cellpadding="3" cellspacing="0">
			<tr>
				<td>
					<u>
						<strong>President</strong>
					</u>
				</td>
				<td align="center">
					<u>
						<strong>Years In Office</strong>
					</u>
				</td>
				<td style="width: 10px"/>
				<td style="background-color: silver; width: 1px"/>
				<td style="width: 10px"/>
				<td>
					<u>
						<strong>President</strong>
					</u>
				</td>
				<td>
					<u>
						<strong>Years In Office</strong>
					</u>
				</td>
			</tr>
			<xsl:for-each select="NewDataSet/Table">
				<xsl:if test="position() mod 2 = 1">
					<xsl:text disable-output-escaping="yes">&lt;tr&gt;</xsl:text>
				</xsl:if>
				<td>
						<xsl:value-of select="Name"/>
				</td>
				<td align="center">
						<xsl:value-of select="YearsInOffice"/>
				</td>

				<xsl:if test="position() mod 2 = 1">
					<td style="width: 10px"/>
					<td style="background-color: silver; width: 1px"/>
					<td style="width: 10px"/>
				</xsl:if>

				<xsl:if test="position() mod 2 = 0">
					<xsl:text disable-output-escaping="yes">&lt;/tr&gt;</xsl:text>
				</xsl:if>
			</xsl:for-each>
		</table>
	</xsl:template>
</xsl:stylesheet>
