cmake_minimum_required(VERSION 3.20)

project(ecod LANGUAGES C)

add_executable(ecod main.c sdo/sdo_list.c)
target_include_directories(ecod PRIVATE
	$<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}>
)