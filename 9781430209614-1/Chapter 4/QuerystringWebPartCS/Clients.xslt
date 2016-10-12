<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<!-- Display each matching client -->
		<table cellpadding="10" border="1">
		<xsl:for-each select="Clients/Clients">
			<tr>
				<td>
					Client Id:
				</td>
				<td>
					<strong>
						<xsl:value-of select="ClientId"/>
					</strong>
				</td>
			</tr>
			<tr>
				<td>
					Client Name: 
				</td>
				<td>
					<strong>
						<xsl:value-of select="ClientName"/>
					</strong>
				</td>
			</tr>
			<tr>
				<td>
					Address: 
				</td>
				<td>
					<strong>
						<xsl:value-of select="Address"/>
					</strong>
				</td>
			</tr>
		</xsl:for-each>
		</table>
	</xsl:template>
</xsl:stylesheet>