-- Generated By protoc-gen-lua Do not Edit
local protobuf = require "protobuf"
module('cs_accelerate_pb')


local ACCELERATEWARNNOTIFY = protobuf.Descriptor();
local ACCELERATEWARNNOTIFY_ACCECOUNT_FIELD = protobuf.FieldDescriptor();

ACCELERATEWARNNOTIFY_ACCECOUNT_FIELD.name = "AcceCount"
ACCELERATEWARNNOTIFY_ACCECOUNT_FIELD.full_name = ".cs.AccelerateWarnNotify.AcceCount"
ACCELERATEWARNNOTIFY_ACCECOUNT_FIELD.number = 1
ACCELERATEWARNNOTIFY_ACCECOUNT_FIELD.index = 0
ACCELERATEWARNNOTIFY_ACCECOUNT_FIELD.label = 1
ACCELERATEWARNNOTIFY_ACCECOUNT_FIELD.has_default_value = false
ACCELERATEWARNNOTIFY_ACCECOUNT_FIELD.default_value = 0
ACCELERATEWARNNOTIFY_ACCECOUNT_FIELD.type = 13
ACCELERATEWARNNOTIFY_ACCECOUNT_FIELD.cpp_type = 3

ACCELERATEWARNNOTIFY.name = "AccelerateWarnNotify"
ACCELERATEWARNNOTIFY.full_name = ".cs.AccelerateWarnNotify"
ACCELERATEWARNNOTIFY.nested_types = {}
ACCELERATEWARNNOTIFY.enum_types = {}
ACCELERATEWARNNOTIFY.fields = {ACCELERATEWARNNOTIFY_ACCECOUNT_FIELD}
ACCELERATEWARNNOTIFY.is_extendable = false
ACCELERATEWARNNOTIFY.extensions = {}

AccelerateWarnNotify = protobuf.Message(ACCELERATEWARNNOTIFY)

