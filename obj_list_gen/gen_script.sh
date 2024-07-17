#!/bin/sh
xscgen -n "|EtherCATInfo.xsd=EsiInfo" \
	-n "|EtherCATBase.xsd=EsiBase" \
	-n "|EtherCATModule.xsd=EsiModule"  \
	-n "|EtherCATDiag.xsd=EsiDiag" \
	-n "|EtherCATDict.xsd=EsiDict" \
	/home/xpan/repos/ecod/obj_list_gen/schemas/EtherCATInfo.xsd \
	/home/xpan/repos/ecod/obj_list_gen/schemas/EtherCATBase.xsd \
	/home/xpan/repos/ecod/obj_list_gen/schemas/EtherCATModule.xsd \
	/home/xpan/repos/ecod/obj_list_gen/schemas/EtherCATDict.xsd \
	/home/xpan/repos/ecod/obj_list_gen/schemas/EtherCATDiag.xsd -o ./src/generated
