<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html"/>

	<xsl:template match="/">
		<table>
			<tr>
				<td width="5%">
					<strong>
						<u>ID</u>
					</strong>
				</td>
				<td width="45%">
					<strong>
						<u>File Name</u>
					</strong>
				</td>
				<td align="right" width="10%">
					<strong>
						<u>Size</u>
					</strong>
				</td>
				<td/>
			</tr>
			<xsl:for-each select="DocumentElement/ListItems">
				<tr>
					<td>
						<xsl:value-of select="ID"/>
					</td>
					<td>
						<xsl:value-of select="FileName"/>
					</td>
					<td align="right">
						<xsl:value-of select="format-number(FileSize,' #,###')"/>
					</td>
					<td/>
				</tr>
			</xsl:for-each>
			<tr>
				<td/>
				<td/>
				<td align="right">---------------</td>
				<td/>
			</tr>
			<tr>
				<td/>
				<td align="center">Total Bytes:</td>
				<td align="right"><xsl:value-of select="format-number(sum(DocumentElement/ListItems/FileSize),'#,###')"/></td>
				<td/>
			</tr>
		</table>
	</xsl:template>
</xsl:stylesheet><!-- Stylus Studio meta-information - (c) 2004-2006. Progress Software Corporation. All rights reserved.
<metaInformation>
<scenarios ><scenario default="yes" name="Scenario1" userelativepaths="yes" externalpreview="no" url="Recipes.xml" htmlbaseurl="" outputurl="" processortype="internal" useresolver="yes" profilemode="0" profiledepth="" profilelength="" urlprofilexml="" commandline="" additionalpath="" additionalclasspath="" postprocessortype="none" postprocesscommandline="" postprocessadditionalpath="" postprocessgeneratedext="" validateoutput="no" validator="internal" customvalidator=""/></scenarios><MapperMetaTag><MapperInfo srcSchemaPathIsRelative="yes" srcSchemaInterpretAsXML="no" destSchemaPath="" destSchemaRoot="" destSchemaPathIsRelative="yes" destSchemaInterpretAsXML="no"/><MapperBlockPosition></MapperBlockPosition><TemplateContext></TemplateContext><MapperFilter side="source"></MapperFilter></MapperMetaTag>
</metaInformation>
-->