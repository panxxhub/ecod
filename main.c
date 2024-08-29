#include <sdo/sdo_list.h>
#include <stdio.h>
#include <strings.h>
#include <sys/stat.h>
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
  struct stat file_stat;
  // stat file
  stat(SDO_FILE_NAME, &file_stat);

  // assert(file_stat.st_size == SDO_NON_VOLATILE_FILE_SIZE,
  //         "File size is not correct");
  if (file_stat.st_size != SDO_NON_VOLATILE_FILE_SIZE) {
    // printf("File size is not correct\n");
    printf("File size is not correct, %ld neq %d\n", file_stat.st_size,
           SDO_NON_VOLATILE_FILE_SIZE);
  }
  printf("din reverse %s\n", x_1st_current_adc_din_reverse ? "true" : "false");

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

  int ffs_0 = ffs(0);
  int ffs_1 = ffs(1);
  int ffs_2 = ffs(2);
  int ffs_64 = ffs(64);

  printf("ffs_0: %d, ffs_1: %d, ffs_2: %d, ffs_64: %d\n", ffs_0, ffs_1, ffs_2,
         ffs_64);

  return 0;
}
