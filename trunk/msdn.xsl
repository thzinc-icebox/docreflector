<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/documentation">
		<html>
			<head>
				Documentation
			</head>
			<body>
				<header>
					<h1>
						Generated Documentation
					</h1>
				</header>
				<nav>
					<ul>
						<li>
							Assemblies
							<ul>
								<xsl:for-each select="assembly">
									<xsl:variable name="assembly" select="."/>
									<li>
										<a>
											<xsl:attribute name="href">
												#A:<xsl:value-of select="fullName"/>
											</xsl:attribute>
											<xsl:value-of select="name"/>
										</a>
										<ul>
											<xsl:for-each select="namespaces/namespace">
												<xsl:variable name="namespace" select="."/>
												<li>
													<a>
														<xsl:attribute name="href">
															#N:<xsl:value-of select="."/>
														</xsl:attribute>
														<xsl:value-of select="."/>
													</a>
													<ul>
														<xsl:for-each select="$assembly/types/type[namespace = $namespace]">
															<li>
																<a>
																	<xsl:attribute name="href">
																		#T:<xsl:value-of select="fullName"/>
																	</xsl:attribute>
																	<xsl:value-of select="name"/>
																</a>
															</li>
														</xsl:for-each>
													</ul>
												</li>
											</xsl:for-each>
										</ul>
									</li>
								</xsl:for-each>
							</ul>
						</li>
					</ul>
				</nav>
				<article id="root" class="active">
					<h1>Assemblies</h1>
					
					<table>
						<thead>
							<th>Assembly Name</th>
							<th>Summary</th>
						</thead>
						<tbody>
							<xsl:for-each select="assembly">
								<tr>
									<td>
										<a>
											<xsl:attribute name="href">
												#A:<xsl:value-of select="fullName"/>
											</xsl:attribute>
											<xsl:value-of select="name"/>
										</a>
									</td>
									<td>
										<xsl:value-of select="summary"/>
									</td>
								</tr>
							</xsl:for-each>
						</tbody>
					</table>
				</article>
				<xsl:for-each select="assembly">
					<article>
						<xsl:attribute name="id">
							A:<xsl:value-of select="fullName"/>
						</xsl:attribute>
						
						<h1>
							<xsl:value-of select="name"/>
						</h1>
						
						<xsl:value-of select="summary"/>
						
						<h2>Namespaces</h2>
						
						<table>
							<thead>
								<th>Namespace</th>
								<th>Summary</th>
							</thead>
							<tbody>
								<xsl:for-each select="namespaces/namespace">
									<td>
										<a>
											<xsl:attribute name="href">
												#N:<xsl:value-of select="."/>
											</xsl:attribute>
											<xsl:value-of select="."/>
										</a>
									</td>
									<td>
										<xsl:value-of select="summary"/>
									</td>
								</xsl:for-each>
							</tbody>
						</table>
					</article>
				</xsl:for-each>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>