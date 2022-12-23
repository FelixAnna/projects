import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:signalr_demo_app/models/chat_member.dart';

import '../controllers/chatDetailController.dart';
import '../controllers/groupChatDetailController.dart';
import '../models/user.dart';
import 'chat_details_page.dart';
import 'group_chat_details_page.dart';

class ConversationItem extends StatefulWidget {
  final ChatMember member;
  final bool isMessageRead;
  final User profile;
  ConversationItem({
    required this.member,
    required this.isMessageRead,
    required this.profile,
  });

  @override
  _ConversationItemState createState() => _ConversationItemState();
}

class _ConversationItemState extends State<ConversationItem> {
  @override
  void initState() {
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: () {
        if (widget.member.type == "user") {
          var chatController = ChatDetailController(
            chatId: widget.member.talkingTo,
            name: widget.member.name,
          );
          chatController.subscribe(Get.find());

          Get.put(chatController, tag: widget.member.talkingTo);
          Get.to(() => ChatDetailsPage(
                chatId: widget.member.talkingTo,
                name: widget.member.name,
              ));
        } else {
          var chatController = GroupChatDetailController(
            chatId: widget.member.talkingTo,
            name: widget.member.name,
          );
          chatController.subscribe(Get.find());

          Get.put(chatController, tag: widget.member.talkingTo);
          Get.to(() => GroupChatDetailsPage(
                chatId: widget.member.talkingTo,
                name: widget.member.name,
              ));
        }
      },
      child: Container(
        padding: EdgeInsets.only(left: 16, right: 16, top: 10, bottom: 10),
        child: Row(
          children: <Widget>[
            Expanded(
              child: Row(
                children: <Widget>[
                  CircleAvatar(
                    backgroundImage: NetworkImage(widget.member.imageURL),
                    maxRadius: 30,
                  ),
                  SizedBox(
                    width: 16,
                  ),
                  Expanded(
                    child: Container(
                      color: Colors.transparent,
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: <Widget>[
                          Text(
                            widget.member.name,
                            style: TextStyle(fontSize: 16),
                          ),
                          SizedBox(
                            height: 6,
                          ),
                          Text(
                            widget.member.messageText,
                            style: TextStyle(
                                fontSize: 13,
                                color: Colors.grey.shade600,
                                fontWeight: widget.isMessageRead
                                    ? FontWeight.bold
                                    : FontWeight.normal),
                          ),
                        ],
                      ),
                    ),
                  ),
                ],
              ),
            ),
            Text(
              widget.member.time,
              style: TextStyle(
                  fontSize: 12,
                  fontWeight: widget.isMessageRead
                      ? FontWeight.bold
                      : FontWeight.normal),
            ),
            VerticalDivider(),
            Text(
              widget.member.unread.toString(),
              style: TextStyle(
                  fontSize: 14,
                  backgroundColor: Colors.lightBlue,
                  fontWeight: widget.isMessageRead
                      ? FontWeight.bold
                      : FontWeight.normal),
            ),
          ],
        ),
      ),
    );
  }
}
