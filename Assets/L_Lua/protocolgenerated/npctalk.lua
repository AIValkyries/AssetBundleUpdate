-- Generated By protoc-gen-lua Do not Edit
local protobuf = require "protobuf"
module('cs_npctalk_pb')


local CSNPCTALKNOTIFY = protobuf.Descriptor();
local CSNPCTALKNOTIFY_TALKID_FIELD = protobuf.FieldDescriptor();

CSNPCTALKNOTIFY_TALKID_FIELD.name = "TalkID"
CSNPCTALKNOTIFY_TALKID_FIELD.full_name = ".cs.CSNpcTalkNotify.TalkID"
CSNPCTALKNOTIFY_TALKID_FIELD.number = 2
CSNPCTALKNOTIFY_TALKID_FIELD.index = 0
CSNPCTALKNOTIFY_TALKID_FIELD.label = 2
CSNPCTALKNOTIFY_TALKID_FIELD.has_default_value = false
CSNPCTALKNOTIFY_TALKID_FIELD.default_value = 0
CSNPCTALKNOTIFY_TALKID_FIELD.type = 13
CSNPCTALKNOTIFY_TALKID_FIELD.cpp_type = 3

CSNPCTALKNOTIFY.name = "CSNpcTalkNotify"
CSNPCTALKNOTIFY.full_name = ".cs.CSNpcTalkNotify"
CSNPCTALKNOTIFY.nested_types = {}
CSNPCTALKNOTIFY.enum_types = {}
CSNPCTALKNOTIFY.fields = {CSNPCTALKNOTIFY_TALKID_FIELD}
CSNPCTALKNOTIFY.is_extendable = false
CSNPCTALKNOTIFY.extensions = {}

CSNpcTalkNotify = protobuf.Message(CSNPCTALKNOTIFY)

