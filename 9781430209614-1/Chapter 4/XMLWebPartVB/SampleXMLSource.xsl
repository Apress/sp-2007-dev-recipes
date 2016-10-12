<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html"/>

	<xsl:template match="/">
		<xsl:for-each select="CustomerData/Customer">
			<h1>Orders for <xsl:value-of select="Name"/></h1>Address: <xsl:value-of select="Address"/><br/>
		Phone: <xsl:value-of select="Phone"/><br/><br/>

			<table cellpadding="3" cellspacing="3">
				<tr valign="bottom">
					<td>
						<strong>
							<u>Order #</u>
						</strong>
					</td>
					<td>
						<strong>
							<u>Product</u>
						</strong>
					</td>
					<td align="center">
						<strong>
							<u>Quanty</u>
						</strong>
					</td>
					<td align="right">
						<strong>
							Unit<br/><u>Price</u>
						</strong>
					</td>
					<td align="right">
						<strong>
							Extended<br/><u>Price</u>
						</strong>
					</td>
				</tr>

				<xsl:for-each select="Order">
					<tr>
						<td>
							<xsl:value-of select="OrderNo"/>
						</td>
						<td>
							<xsl:value-of select="Product"/>
						</td>
						<td align="center">
							<xsl:value-of select="Qty"/>
						</td>
						<td align="right">
							<xsl:value-of select="format-number(UnitPrice,'$ #,###')"/>
						</td>
						<td align="right">
							<xsl:value-of select="format-number(ExtPrice,'$ #,###')"/>
						</td>
					</tr>
				</xsl:for-each>
				<tr>
					<td colspan="4"/>
					<td align="right">
						==========
					</td>
				</tr>
				<tr>
					<td colspan="4"/>
					<td align="right">
						<xsl:value-of select="format-number(sum(Order/ExtPrice),'$ #,###')"/>
					</td>
				</tr>
			</table>
		</xsl:for-each>
	</xsl:template>
</xsl:stylesheet>