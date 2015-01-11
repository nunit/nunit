
ASSEMBLY = ../bin/Mono.Addins.dll
ASSEMBLY_NAME=Mono.Addins
PC_FILES_IN = mono-addins.pc.in

install-data-local: gac-install
uninstall-local: gac-uninstall

include $(top_srcdir)/Makefile.include
include $(top_srcdir)/xbuild.include

CLEANFILES += $(POLICY_ASSEMBLIES) $(POLICY_CONFIGS)

