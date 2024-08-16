#include <sdo/sdo_list.h>
#include <stdio.h>
#include <unistd.h>
#define SDO_FILE_NAME "sdo_list.bin"
int main() {
  // struct fs_file_t {
  //    FILE* file_p
  // };
  struct fs_file_t my_file;

  //  open file, create if not exist
  //   my_file.file_p = fopen(SDO_FILE_NAME, "a+");
  // check if the file exists first
  bool file_exists = false;
  if (access(SDO_FILE_NAME, F_OK) != -1) {
    // file exists
    file_exists = true;
    my_file.file_p = fopen(SDO_FILE_NAME, "r+");
  } else {
    // file doesn't exist
    my_file.file_p = fopen(SDO_FILE_NAME, "w+");
  }

  if (file_exists) {
    sdo_objects_init_from_storage(&my_file);
  } else {
    sdo_restore_defaults();
    sdo_objects_save_to_storage(&my_file);
  }

  printf("Number of sm2: %d, 1st vel loop gain %d\n",
         sync_manager_channel_2.number_of_assigned_pdos,
         x_1st_velocity_loop_gain);
  x_1st_velocity_loop_gain = 0;
  printf("Number of sm2: %d, 1st vel loop gain %d\n",
         sync_manager_channel_2.number_of_assigned_pdos,
         x_1st_velocity_loop_gain);
  sdo_object_load_from_storage(&my_file, 0x3101, 0);
  printf("Number of sm2: %d, 1st vel loop gain %d\n",
         sync_manager_channel_2.number_of_assigned_pdos,
         x_1st_velocity_loop_gain);
  x_1st_velocity_loop_gain = 0x42;
  sdo_object_save_to_storage(&my_file, 0x3101, 0);
  x_1st_velocity_loop_gain = 0;

  printf("Number of sm2: %d, 1st vel loop gain %d\n",
         sync_manager_channel_2.number_of_assigned_pdos,
         x_1st_velocity_loop_gain);
  sdo_object_load_from_storage(&my_file, 0x3101, 0);
  printf("Number of sm2: %d, 1st vel loop gain %d\n",
         sync_manager_channel_2.number_of_assigned_pdos,
         x_1st_velocity_loop_gain);

  return 0;
}
