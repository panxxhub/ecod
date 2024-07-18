#ifndef __ZEPHYR_SDO_OBJECTS_H__
#define __ZEPHYR_SDO_OBJECTS_H__

#include <stdint.h>

typedef struct obj_desc {
	uint16_t subindex;
	uint16_t datatype;
	uint16_t bitlength;
	uint16_t flags;
	const char *name;
	uint32_t value;
	uint8_t *data; // used for visible string type
} obj_desc_t;

typedef struct obj_list {
	uint16_t index;
	uint16_t objtype;
	uint8_t maxsub;
	uint8_t pad1;
	const char *name;
	const obj_desc_t *objdesc;
} obj_list_t;

#define OBJH_READ  0
#define OBJH_WRITE 1

#define OTYPE_DOMAIN    0x0002
#define OTYPE_DEFTYPE   0x0005
#define OTYPE_DEFSTRUCT 0x0006
#define OTYPE_VAR       0x0007
#define OTYPE_ARRAY     0x0008
#define OTYPE_RECORD    0x0009

#define DTYPE_BOOLEAN        0x0001
#define DTYPE_INTEGER8       0x0002
#define DTYPE_INTEGER16      0x0003
#define DTYPE_INTEGER32      0x0004
#define DTYPE_UNSIGNED8      0x0005
#define DTYPE_UNSIGNED16     0x0006
#define DTYPE_UNSIGNED32     0x0007
#define DTYPE_REAL32         0x0008
#define DTYPE_VISIBLE_STRING 0x0009
#define DTYPE_OCTET_STRING   0x000A
#define DTYPE_UNICODE_STRING 0x000B
#define DTYPE_INTEGER24      0x0010
#define DTYPE_UNSIGNED24     0x0016
#define DTYPE_INTEGER64      0x0015
#define DTYPE_UNSIGNED64     0x001B
#define DTYPE_REAL64         0x0011
#define DTYPE_PDO_MAPPING    0x0021
#define DTYPE_IDENTITY       0x0023
#define DTYPE_BITARR8        0x002D
#define DTYPE_BITARR16       0x002E
#define DTYPE_BITARR32       0x002F
#define DTYPE_BIT1           0x0030
#define DTYPE_BIT2           0x0031
#define DTYPE_BIT3           0x0032
#define DTYPE_BIT4           0x0033
#define DTYPE_BIT5           0x0034
#define DTYPE_BIT6           0x0035
#define DTYPE_BIT7           0x0036
#define DTYPE_BIT8           0x0037
#define DTYPE_ARRAY_OF_INT   0x0260
#define DTYPE_ARRAY_OF_SINT  0x0261
#define DTYPE_ARRAY_OF_DINT  0x0262
#define DTYPE_ARRAY_OF_UDINT 0x0263

#define ATYPE_Rpre    0x01
#define ATYPE_Rsafe   0x02
#define ATYPE_Rop     0x04
#define ATYPE_Wpre    0x08
#define ATYPE_Wsafe   0x10
#define ATYPE_Wop     0x20
#define ATYPE_RXPDO   0x40
#define ATYPE_TXPDO   0x80
#define ATYPE_BACKUP  0x100
#define ATYPE_SETTING 0x200

#define ATYPE_RO         (ATYPE_Rpre | ATYPE_Rsafe | ATYPE_Rop)
#define ATYPE_WO         (ATYPE_Wpre | ATYPE_Wsafe | ATYPE_Wop)
#define ATYPE_RW         (ATYPE_RO | ATYPE_WO)
#define ATYPE_RWpre      (ATYPE_Wpre | ATYPE_RO)
#define ATYPE_RWop       (ATYPE_Wop | ATYPE_RO)
#define ATYPE_RWpre_safe (ATYPE_Wpre | ATYPE_Wsafe | ATYPE_RO)

/******************************************************************************************
 *  Object Definitions
 *****************************************************************************************/

typedef struct identity_object {
	uint32_t vendor_id;
	uint32_t product_code;
	uint32_t revision_number;
	uint32_t serial_number;
} identity_object_t;

typedef struct analog_servo_parameters {
	int32_t analog_input_gain;
	int16_t analog_input_polarity;
	int32_t analog_input_integration_time_constant;
	int32_t analog_input_integration_limit;
	int16_t analog_input_offset;
	int16_t analog_input_filter;
	int16_t analog_input_excess_setup;
} analog_servo_parameters_t;

typedef struct interpolation_time_period {
	uint8_t interpolation_time_period_value;
	int8_t interpolation_time_index;
} interpolation_time_period_t;


typedef char diagnosis_msg[16];
typedef struct diagnosis_history {
	uint8_t newest_msg;
	uint8_t newest_ack_msg
	bool newest_msg_avail;
	uint16_t flags;
	diagnosis_msg msg_group[14];
} diagnosis_history_t;

typedef struct pdo_mapping {
	uint8_t number_of_entries;
	uint32_t pdo_mapped[32];
} pdo_mapping_t;

typedef struct sm_sync {
	uint8_t sync_mode;
	uint32_t cycle_time;
	uint32_t shift_time;
	uint16_t sync_modes_supported;
	uint32_t min_cycle_time;
	uint32_t calc_and_copy_time;
	uint16_t command;
	uint32_t delay_time;
	uint32_t sync0_cycle_time;
	uint16_t cycle_time_too_small;
	uint16_t sm_event_missed;
	uint16_t shift_time_too_short;
	uint16_t rxpdo_toggle_failed;
	bool sync_error;
} sm_sync_t;

/******************************************************************************************
 *  Object list(s) for the SDO server
 *****************************************************************************************/
extern const obj_list_t sdo_objects[];
extern const pdo_mapping_t  rxpdo_mapping_0;
extern const pdo_mapping_t  rxpdo_mapping_1;
extern const pdo_mapping_t  rxpdo_mapping_2;
extern const pdo_mapping_t  rxpdo_mapping_3;
extern const pdo_mapping_t  txpdo_mapping_0;
extern const pdo_mapping_t  txpdo_mapping_1;
extern const pdo_mapping_t  txpdo_mapping_2;
extern const pdo_mapping_t  txpdo_mapping_3;
extern const sm_sync_t  sm2_sync;
extern const sm_sync_t  sm3_sync;
extern const identity_object_t  identity;
extern const diagnosis_history_t  diagnosis_history;
extern const analog_servo_parameters_t  analog_servo_parameters;
extern const interpolation_time_period_t  interpolation_time_period;
#endif // __ZEPHYR_SDO_OBJECTS_H__