<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxml="urn:schemas-microsoft-com:xslt">
<xsl:output method="html" indent="no"/>

<xsl:template match="/">

	<xsl:variable name="cov0style" select="'background:#E79090;text-align:right;'"/>
	<xsl:variable name="cov20style" select="'background:#D79797;text-align:right;'"/>
	<xsl:variable name="cov40style" select="'background:#D7A0A0;text-align:right;'"/>
	<xsl:variable name="cov60style" select="'background:#C7A7A7;text-align:right;'"/>
	<xsl:variable name="cov80style" select="'background:#C0B0B0;text-align:right;'"/>
	<xsl:variable name="cov100style" select="'background:#D7D7D7;text-align:right;'"/>
	
	<table style="border-collapse: collapse;">
		<tr style="font-weight:bold; background:whitesmoke;"><td colspan="2">Coverage by assembly</td></tr>
		
		<xsl:variable name="unique-asms" select="/PartCoverReport[@version='4.0']/Type[not(@asmref=following::Type/@asmref)]"/>
		<xsl:for-each select="$unique-asms">
			<xsl:variable name="current-asm" select="./@asmref"/>
			<tr>
				
				<xsl:element name="td">
					<xsl:attribute name="style">background:ghostwhite; padding: 5px  30px 5px  5px;</xsl:attribute>
					<xsl:value-of select="//Assembly[@id=$current-asm]/@name"/>
				</xsl:element>
				
				<xsl:variable name="codeSize" select="sum(//Type[@asmref=$current-asm]/Method/pt/@len)+sum(//Type[@asmref=$current-asm]/Method[count(pt)=0]/@bodysize)"/>
				<xsl:variable name="coveredCodeSize" select="sum(//Type[@asmref=$current-asm]/Method/pt[@visit>0]/@len)"/>
				
				<xsl:element name="td">
					<xsl:if test="$codeSize=0">
						<xsl:attribute name="style"><xsl:value-of select="$cov0style"/></xsl:attribute>
						0%
					</xsl:if>

					<xsl:if test="$codeSize &gt; 0">
						<xsl:variable name="coverage" select="round(100 * $coveredCodeSize div $codeSize)"/>
						
						<xsl:if test="$coverage &gt;=  0 and $coverage &lt; 20"><xsl:attribute name="style"><xsl:value-of select="$cov20style"/></xsl:attribute></xsl:if>
						<xsl:if test="$coverage &gt;= 20 and $coverage &lt; 40"><xsl:attribute name="style"><xsl:value-of select="$cov40style"/></xsl:attribute></xsl:if>
						<xsl:if test="$coverage &gt;= 40 and $coverage &lt; 60"><xsl:attribute name="style"><xsl:value-of select="$cov60style"/></xsl:attribute></xsl:if>
						<xsl:if test="$coverage &gt;= 60 and $coverage &lt; 80"><xsl:attribute name="style"><xsl:value-of select="$cov80style"/></xsl:attribute></xsl:if>
						<xsl:if test="$coverage &gt;= 80"><xsl:attribute name="style"><xsl:value-of select="$cov100style"/></xsl:attribute></xsl:if>
						<xsl:value-of select="$coverage"/>%
					</xsl:if>
					
				</xsl:element>
			</tr>
		</xsl:for-each>
	</table>
	
</xsl:template>

</xsl:stylesheet>
