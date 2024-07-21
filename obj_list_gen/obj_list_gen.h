// clang-format off

/**
 * THIS FILE IS AUTO GENERATED 
 * GENERATED AT 2024-07-21 14:41:53
 * !!! DO NOT EDIT IT !!!
 */

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
 *  EXPOSED OBJECTS DEFINITIONS
 *****************************************************************************************/
typedef int16_t hysteresis_at_torque_control_switching_t; 
typedef int16_t x_1st_damping_frequency_t; 
typedef int16_t x_2nd_damping_filter_setup_t; 
typedef int32_t positioning_complete_in_position_range_t; 
typedef int16_t function_expansion_setup_2_t; 
typedef int16_t tuning_filter_t; 
typedef char drive_serial_number_t[10]; 

typedef struct identity_object {
  uint32_t vendor_id;
  uint32_t product_code;
  uint32_t revision_number;
  uint32_t serial_number;
} identity_object_t;

typedef struct diagnosis_history {
  uint8_t maximum_messages;
  uint8_t newest_message;
  uint8_t newest_acknowledged_message;
  bool new_messages_available;
  uint16_t flags;
  char diagnosis_message_1[16];
  char diagnosis_message_2[16];
  char diagnosis_message_3[16];
  char diagnosis_message_4[16];
  char diagnosis_message_5[16];
  char diagnosis_message_6[16];
  char diagnosis_message_7[16];
  char diagnosis_message_8[16];
  char diagnosis_message_9[16];
  char diagnosis_message_10[16];
  char diagnosis_message_11[16];
  char diagnosis_message_12[16];
  char diagnosis_message_13[16];
  char diagnosis_message_14[16];
} diagnosis_history_t;

typedef struct receive_pdo_mapping_1 {
  uint8_t number_of_entries;
  uint32_t x_1st_receive_pdo_mapped;
  uint32_t x_2nd_receive_pdo_mapped;
  uint32_t x_3rd_receive_pdo_mapped;
  uint32_t x_4th_receive_pdo_mapped;
  uint32_t x_5th_receive_pdo_mapped;
  uint32_t x_6th_receive_pdo_mapped;
  uint32_t x_7th_receive_pdo_mapped;
  uint32_t x_8th_receive_pdo_mapped;
  uint32_t x_9th_receive_pdo_mapped;
  uint32_t x_10th_receive_pdo_mapped;
  uint32_t x_11th_receive_pdo_mapped;
  uint32_t x_12th_receive_pdo_mapped;
  uint32_t x_13th_receive_pdo_mapped;
  uint32_t x_14th_receive_pdo_mapped;
  uint32_t x_15th_receive_pdo_mapped;
  uint32_t x_16th_receive_pdo_mapped;
  uint32_t x_17th_receive_pdo_mapped;
  uint32_t x_18th_receive_pdo_mapped;
  uint32_t x_19th_receive_pdo_mapped;
  uint32_t x_20th_receive_pdo_mapped;
  uint32_t x_21st_receive_pdo_mapped;
  uint32_t x_22nd_receive_pdo_mapped;
  uint32_t x_23rd_receive_pdo_mapped;
  uint32_t x_24th_receive_pdo_mapped;
  uint32_t x_25th_receive_pdo_mapped;
  uint32_t x_26th_receive_pdo_mapped;
  uint32_t x_27th_receive_pdo_mapped;
  uint32_t x_28th_receive_pdo_mapped;
  uint32_t x_29th_receive_pdo_mapped;
  uint32_t x_30th_receive_pdo_mapped;
  uint32_t x_31st_receive_pdo_mapped;
  uint32_t x_32nd_receive_pdo_mapped;
} receive_pdo_mapping_1_t;

typedef struct receive_pdo_mapping_2 {
  uint8_t number_of_entries;
  uint32_t x_1st_receive_pdo_mapped;
  uint32_t x_2nd_receive_pdo_mapped;
  uint32_t x_3rd_receive_pdo_mapped;
  uint32_t x_4th_receive_pdo_mapped;
  uint32_t x_5th_receive_pdo_mapped;
  uint32_t x_6th_receive_pdo_mapped;
  uint32_t x_7th_receive_pdo_mapped;
  uint32_t x_8th_receive_pdo_mapped;
  uint32_t x_9th_receive_pdo_mapped;
  uint32_t x_10th_receive_pdo_mapped;
  uint32_t x_11th_receive_pdo_mapped;
  uint32_t x_12th_receive_pdo_mapped;
  uint32_t x_13th_receive_pdo_mapped;
  uint32_t x_14th_receive_pdo_mapped;
  uint32_t x_15th_receive_pdo_mapped;
  uint32_t x_16th_receive_pdo_mapped;
  uint32_t x_17th_receive_pdo_mapped;
  uint32_t x_18th_receive_pdo_mapped;
  uint32_t x_19th_receive_pdo_mapped;
  uint32_t x_20th_receive_pdo_mapped;
  uint32_t x_21st_receive_pdo_mapped;
  uint32_t x_22nd_receive_pdo_mapped;
  uint32_t x_23rd_receive_pdo_mapped;
  uint32_t x_24th_receive_pdo_mapped;
  uint32_t x_25th_receive_pdo_mapped;
  uint32_t x_26th_receive_pdo_mapped;
  uint32_t x_27th_receive_pdo_mapped;
  uint32_t x_28th_receive_pdo_mapped;
  uint32_t x_29th_receive_pdo_mapped;
  uint32_t x_30th_receive_pdo_mapped;
  uint32_t x_31st_receive_pdo_mapped;
  uint32_t x_32nd_receive_pdo_mapped;
} receive_pdo_mapping_2_t;

typedef struct receive_pdo_mapping_3 {
  uint8_t number_of_entries;
  uint32_t x_1st_receive_pdo_mapped;
  uint32_t x_2nd_receive_pdo_mapped;
  uint32_t x_3rd_receive_pdo_mapped;
  uint32_t x_4th_receive_pdo_mapped;
  uint32_t x_5th_receive_pdo_mapped;
  uint32_t x_6th_receive_pdo_mapped;
  uint32_t x_7th_receive_pdo_mapped;
  uint32_t x_8th_receive_pdo_mapped;
  uint32_t x_9th_receive_pdo_mapped;
  uint32_t x_10th_receive_pdo_mapped;
  uint32_t x_11th_receive_pdo_mapped;
  uint32_t x_12th_receive_pdo_mapped;
  uint32_t x_13th_receive_pdo_mapped;
  uint32_t x_14th_receive_pdo_mapped;
  uint32_t x_15th_receive_pdo_mapped;
  uint32_t x_16th_receive_pdo_mapped;
  uint32_t x_17th_receive_pdo_mapped;
  uint32_t x_18th_receive_pdo_mapped;
  uint32_t x_19th_receive_pdo_mapped;
  uint32_t x_20th_receive_pdo_mapped;
  uint32_t x_21st_receive_pdo_mapped;
  uint32_t x_22nd_receive_pdo_mapped;
  uint32_t x_23rd_receive_pdo_mapped;
  uint32_t x_24th_receive_pdo_mapped;
  uint32_t x_25th_receive_pdo_mapped;
  uint32_t x_26th_receive_pdo_mapped;
  uint32_t x_27th_receive_pdo_mapped;
  uint32_t x_28th_receive_pdo_mapped;
  uint32_t x_29th_receive_pdo_mapped;
  uint32_t x_30th_receive_pdo_mapped;
  uint32_t x_31st_receive_pdo_mapped;
  uint32_t x_32nd_receive_pdo_mapped;
} receive_pdo_mapping_3_t;

typedef struct receive_pdo_mapping_4 {
  uint8_t number_of_entries;
  uint32_t x_1st_receive_pdo_mapped;
  uint32_t x_2nd_receive_pdo_mapped;
  uint32_t x_3rd_receive_pdo_mapped;
  uint32_t x_4th_receive_pdo_mapped;
  uint32_t x_5th_receive_pdo_mapped;
  uint32_t x_6th_receive_pdo_mapped;
  uint32_t x_7th_receive_pdo_mapped;
  uint32_t x_8th_receive_pdo_mapped;
  uint32_t x_9th_receive_pdo_mapped;
  uint32_t x_10th_receive_pdo_mapped;
  uint32_t x_11th_receive_pdo_mapped;
  uint32_t x_12th_receive_pdo_mapped;
  uint32_t x_13th_receive_pdo_mapped;
  uint32_t x_14th_receive_pdo_mapped;
  uint32_t x_15th_receive_pdo_mapped;
  uint32_t x_16th_receive_pdo_mapped;
  uint32_t x_17th_receive_pdo_mapped;
  uint32_t x_18th_receive_pdo_mapped;
  uint32_t x_19th_receive_pdo_mapped;
  uint32_t x_20th_receive_pdo_mapped;
  uint32_t x_21st_receive_pdo_mapped;
  uint32_t x_22nd_receive_pdo_mapped;
  uint32_t x_23rd_receive_pdo_mapped;
  uint32_t x_24th_receive_pdo_mapped;
  uint32_t x_25th_receive_pdo_mapped;
  uint32_t x_26th_receive_pdo_mapped;
  uint32_t x_27th_receive_pdo_mapped;
  uint32_t x_28th_receive_pdo_mapped;
  uint32_t x_29th_receive_pdo_mapped;
  uint32_t x_30th_receive_pdo_mapped;
  uint32_t x_31st_receive_pdo_mapped;
  uint32_t x_32nd_receive_pdo_mapped;
} receive_pdo_mapping_4_t;

typedef struct transmit_pdo_mapping_1 {
  uint8_t number_of_entries;
  uint32_t x_1st_transmit_pdo_mapped;
  uint32_t x_2nd_transmit_pdo_mapped;
  uint32_t x_3rd_transmit_pdo_mapped;
  uint32_t x_4th_transmit_pdo_mapped;
  uint32_t x_5th_transmit_pdo_mapped;
  uint32_t x_6th_transmit_pdo_mapped;
  uint32_t x_7th_transmit_pdo_mapped;
  uint32_t x_8th_transmit_pdo_mapped;
  uint32_t x_9th_transmit_pdo_mapped;
  uint32_t x_10th_transmit_pdo_mapped;
  uint32_t x_11th_transmit_pdo_mapped;
  uint32_t x_12th_transmit_pdo_mapped;
  uint32_t x_13th_transmit_pdo_mapped;
  uint32_t x_14th_transmit_pdo_mapped;
  uint32_t x_15th_transmit_pdo_mapped;
  uint32_t x_16th_transmit_pdo_mapped;
  uint32_t x_17th_transmit_pdo_mapped;
  uint32_t x_18th_transmit_pdo_mapped;
  uint32_t x_19th_transmit_pdo_mapped;
  uint32_t x_20th_transmit_pdo_mapped;
  uint32_t x_21st_transmit_pdo_mapped;
  uint32_t x_22nd_transmit_pdo_mapped;
  uint32_t x_23rd_transmit_pdo_mapped;
  uint32_t x_24th_transmit_pdo_mapped;
  uint32_t x_25th_transmit_pdo_mapped;
  uint32_t x_26th_transmit_pdo_mapped;
  uint32_t x_27th_transmit_pdo_mapped;
  uint32_t x_28th_transmit_pdo_mapped;
  uint32_t x_29th_transmit_pdo_mapped;
  uint32_t x_30th_transmit_pdo_mapped;
  uint32_t x_31st_transmit_pdo_mapped;
  uint32_t x_32nd_transmit_pdo_mapped;
} transmit_pdo_mapping_1_t;

typedef struct transmit_pdo_mapping_2 {
  uint8_t number_of_entries;
  uint32_t x_1st_transmit_pdo_mapped;
  uint32_t x_2nd_transmit_pdo_mapped;
  uint32_t x_3rd_transmit_pdo_mapped;
  uint32_t x_4th_transmit_pdo_mapped;
  uint32_t x_5th_transmit_pdo_mapped;
  uint32_t x_6th_transmit_pdo_mapped;
  uint32_t x_7th_transmit_pdo_mapped;
  uint32_t x_8th_transmit_pdo_mapped;
  uint32_t x_9th_transmit_pdo_mapped;
  uint32_t x_10th_transmit_pdo_mapped;
  uint32_t x_11th_transmit_pdo_mapped;
  uint32_t x_12th_transmit_pdo_mapped;
  uint32_t x_13th_transmit_pdo_mapped;
  uint32_t x_14th_transmit_pdo_mapped;
  uint32_t x_15th_transmit_pdo_mapped;
  uint32_t x_16th_transmit_pdo_mapped;
  uint32_t x_17th_transmit_pdo_mapped;
  uint32_t x_18th_transmit_pdo_mapped;
  uint32_t x_19th_transmit_pdo_mapped;
  uint32_t x_20th_transmit_pdo_mapped;
  uint32_t x_21st_transmit_pdo_mapped;
  uint32_t x_22nd_transmit_pdo_mapped;
  uint32_t x_23rd_transmit_pdo_mapped;
  uint32_t x_24th_transmit_pdo_mapped;
  uint32_t x_25th_transmit_pdo_mapped;
  uint32_t x_26th_transmit_pdo_mapped;
  uint32_t x_27th_transmit_pdo_mapped;
  uint32_t x_28th_transmit_pdo_mapped;
  uint32_t x_29th_transmit_pdo_mapped;
  uint32_t x_30th_transmit_pdo_mapped;
  uint32_t x_31st_transmit_pdo_mapped;
  uint32_t x_32nd_transmit_pdo_mapped;
} transmit_pdo_mapping_2_t;

typedef struct transmit_pdo_mapping_3 {
  uint8_t number_of_entries;
  uint32_t x_1st_transmit_pdo_mapped;
  uint32_t x_2nd_transmit_pdo_mapped;
  uint32_t x_3rd_transmit_pdo_mapped;
  uint32_t x_4th_transmit_pdo_mapped;
  uint32_t x_5th_transmit_pdo_mapped;
  uint32_t x_6th_transmit_pdo_mapped;
  uint32_t x_7th_transmit_pdo_mapped;
  uint32_t x_8th_transmit_pdo_mapped;
  uint32_t x_9th_transmit_pdo_mapped;
  uint32_t x_10th_transmit_pdo_mapped;
  uint32_t x_11th_transmit_pdo_mapped;
  uint32_t x_12th_transmit_pdo_mapped;
  uint32_t x_13th_transmit_pdo_mapped;
  uint32_t x_14th_transmit_pdo_mapped;
  uint32_t x_15th_transmit_pdo_mapped;
  uint32_t x_16th_transmit_pdo_mapped;
  uint32_t x_17th_transmit_pdo_mapped;
  uint32_t x_18th_transmit_pdo_mapped;
  uint32_t x_19th_transmit_pdo_mapped;
  uint32_t x_20th_transmit_pdo_mapped;
  uint32_t x_21st_transmit_pdo_mapped;
  uint32_t x_22nd_transmit_pdo_mapped;
  uint32_t x_23rd_transmit_pdo_mapped;
  uint32_t x_24th_transmit_pdo_mapped;
  uint32_t x_25th_transmit_pdo_mapped;
  uint32_t x_26th_transmit_pdo_mapped;
  uint32_t x_27th_transmit_pdo_mapped;
  uint32_t x_28th_transmit_pdo_mapped;
  uint32_t x_29th_transmit_pdo_mapped;
  uint32_t x_30th_transmit_pdo_mapped;
  uint32_t x_31st_transmit_pdo_mapped;
  uint32_t x_32nd_transmit_pdo_mapped;
} transmit_pdo_mapping_3_t;

typedef struct transmit_pdo_mapping_4 {
  uint8_t number_of_entries;
  uint32_t x_1st_transmit_pdo_mapped;
  uint32_t x_2nd_transmit_pdo_mapped;
  uint32_t x_3rd_transmit_pdo_mapped;
  uint32_t x_4th_transmit_pdo_mapped;
  uint32_t x_5th_transmit_pdo_mapped;
  uint32_t x_6th_transmit_pdo_mapped;
  uint32_t x_7th_transmit_pdo_mapped;
  uint32_t x_8th_transmit_pdo_mapped;
  uint32_t x_9th_transmit_pdo_mapped;
  uint32_t x_10th_transmit_pdo_mapped;
  uint32_t x_11th_transmit_pdo_mapped;
  uint32_t x_12th_transmit_pdo_mapped;
  uint32_t x_13th_transmit_pdo_mapped;
  uint32_t x_14th_transmit_pdo_mapped;
  uint32_t x_15th_transmit_pdo_mapped;
  uint32_t x_16th_transmit_pdo_mapped;
  uint32_t x_17th_transmit_pdo_mapped;
  uint32_t x_18th_transmit_pdo_mapped;
  uint32_t x_19th_transmit_pdo_mapped;
  uint32_t x_20th_transmit_pdo_mapped;
  uint32_t x_21st_transmit_pdo_mapped;
  uint32_t x_22nd_transmit_pdo_mapped;
  uint32_t x_23rd_transmit_pdo_mapped;
  uint32_t x_24th_transmit_pdo_mapped;
  uint32_t x_25th_transmit_pdo_mapped;
  uint32_t x_26th_transmit_pdo_mapped;
  uint32_t x_27th_transmit_pdo_mapped;
  uint32_t x_28th_transmit_pdo_mapped;
  uint32_t x_29th_transmit_pdo_mapped;
  uint32_t x_30th_transmit_pdo_mapped;
  uint32_t x_31st_transmit_pdo_mapped;
  uint32_t x_32nd_transmit_pdo_mapped;
} transmit_pdo_mapping_4_t;

typedef struct sync_manager_2_synchronization {
  uint16_t sync_mode;
  uint32_t cycle_time;
  uint32_t shift_time;
  uint16_t sync_modes_supported;
  uint32_t minimum_cycle_time;
  uint32_t calc_and_copy_time;
  uint16_t command;
  uint32_t delay_time;
  uint32_t sync0_cycle_time;
  uint16_t cycle_time_too_small;
  uint16_t sm_event_missed;
  uint16_t shift_time_too_short;
  uint16_t rxpdo_toggle_failed;
  bool sync_error;
} sync_manager_2_synchronization_t;

typedef struct sync_manager_3_synchronization {
  uint16_t sync_mode;
  uint32_t cycle_time;
  uint32_t shift_time;
  uint16_t sync_modes_supported;
  uint32_t minimum_cycle_time;
  uint32_t calc_and_copy_time;
  uint16_t command;
  uint32_t delay_time;
  uint32_t sync0_cycle_time;
  uint16_t cycle_time_too_small;
  uint16_t sm_event_missed;
  uint16_t shift_time_too_short;
  uint16_t rxpdo_toggle_failed;
  bool sync_error;
} sync_manager_3_synchronization_t;


typedef struct store_parameters {
  uint32_t element[1];
} store_parameters_t;

typedef struct sync_manager_channel_2 {
  uint8_t number_of_assigned_pdos;
  uint16_t element[4];
} sync_manager_channel_2_t;

typedef struct sync_manager_channel_3 {
  uint8_t number_of_assigned_pdos;
  uint16_t element[4];
} sync_manager_channel_3_t;

typedef struct alarm_accessory_information {
  uint8_t number_of_entries;
  uint8_t element[36];
} alarm_accessory_information_t;

typedef struct digital_outputs {
  uint8_t number_of_entries;
  uint32_t element[2];
} digital_outputs_t;




/******************************************************************************************
 *  OBJECT LIST(S) FOR THE SDO SERVER
 *****************************************************************************************/
extern const obj_list_t sdo_objects[];
extern const store_parameters_t store_parameters;
extern const identity_object_t identity_object;
extern const diagnosis_history_t diagnosis_history;
extern const receive_pdo_mapping_1_t receive_pdo_mapping_1;
extern const receive_pdo_mapping_2_t receive_pdo_mapping_2;
extern const receive_pdo_mapping_3_t receive_pdo_mapping_3;
extern const receive_pdo_mapping_4_t receive_pdo_mapping_4;
extern const transmit_pdo_mapping_1_t transmit_pdo_mapping_1;
extern const transmit_pdo_mapping_2_t transmit_pdo_mapping_2;
extern const transmit_pdo_mapping_3_t transmit_pdo_mapping_3;
extern const transmit_pdo_mapping_4_t transmit_pdo_mapping_4;
extern const sync_manager_channel_2_t sync_manager_channel_2;
extern const sync_manager_channel_3_t sync_manager_channel_3;
extern const sync_manager_2_synchronization_t sync_manager_2_synchronization;
extern const sync_manager_3_synchronization_t sync_manager_3_synchronization;
extern hysteresis_at_torque_control_switching_t hysteresis_at_torque_control_switching;
extern x_1st_damping_frequency_t x_1st_damping_frequency;
extern x_2nd_damping_filter_setup_t x_2nd_damping_filter_setup;
extern positioning_complete_in_position_range_t positioning_complete_in_position_range;
extern function_expansion_setup_2_t function_expansion_setup_2;
extern tuning_filter_t tuning_filter;
extern drive_serial_number_t drive_serial_number;
extern const alarm_accessory_information_t alarm_accessory_information;
extern const digital_outputs_t digital_outputs;

/******************************************************************************************
 * SDO_OBJECTS HELPERS
 *****************************************************************************************/
#define SDO_OBJECTS_COUNT 		645
#define SOD_OBJECTS_COUNT_LOG2UP	10

/** Search for an object index matching the wanted value in the Object List.
 * Search in a binary-search fashion.
 * @param[in] index   = value on index of object we want to locate
 * @return local array index if we succeed, -1 if we didn't find the index.
 */
static ALWAYS_INLINE int32_t sdo_find_object(uint16_t index)
{
#define SDO_BINARY_SEARCH_LOW_INIT  0
#define SDO_BINARY_SEARCH_MID_INIT  322
#define SDO_BINARY_SEARCH_HIGH_INIT 644
	int low =  SDO_BINARY_SEARCH_LOW_INIT;
	int high = SDO_BINARY_SEARCH_HIGH_INIT;
	int mid =  SDO_BINARY_SEARCH_MID_INIT;
	for(int iter = 0; iter < SOD_OBJECTS_COUNT_LOG2UP; iter++) {
		if (sdo_objects[mid].index == index) {
			return mid;
		} else if (sdo_objects[mid].index < index) {
			low = mid + 1;
		} else {
			high = mid - 1;
		}
		mid = (low + high) / 2;
	}
	return -1;
}

/** Search for an object sub-index.
 *
 * @param[in] nidx   = local array index of object we want to find sub-index to
 * @param[in] subindex   = value on sub-index of object we want to locate
 * @return local array index if we succeed, -1 if we didn't find the index.
 */
static ALWAYS_INLINE int16_t sdo_find_subindex(int32_t nidx, uint8_t subindex)
{
	const obj_desc_t *objd;
	int16_t n = sdo_find_object(nidx);
	if(n < 0) {
		return -1;
	}
	int16_t max_sub = sdo_objects[n].maxsub;
	int low = 0;
	int high = max_sub - 1;
	int mid = (low + high) / 2;
	for(int iter = 0; iter < 6; iter++) {
		objd = sdo_objects[n].objdesc[mid];
		if (objd->subindex == subindex) {
			return mid;
		} else if (objd->subindex < subindex) {
			low = mid + 1;
		} else {
			high = mid - 1;
		}
		mid = (low + high) / 2;
	}
	return -1;
}

#endif // __ZEPHYR_SDO_OBJECTS_H__
// clang-format on