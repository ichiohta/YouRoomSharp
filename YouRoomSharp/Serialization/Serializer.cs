using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using YouRoomSharp.Data;

namespace YouRoomSharp.Serialization
{
    public static class Serializer
    {
        private static readonly IEnumerable<XElement> EmptyElementSet = Enumerable.Empty<XElement>();

        public static Entry ToEntry(this XElement element)
        {
            Assert.IsNotNull(element, nameof(element));

            return
                new Entry()
                {
                    Id = element.GetInt("id"),
                    Content = element.GetString("content"),
                    CreatedAt = element.GetDateTimeOffset("created-at"),
                    UpdatedAt = element.GetDateTimeOffset("updated-at"),
                    ParentId = element.GetNullableInt("parent-id"),
                    RootId = element.GetInt("root-id"),
                    DescendantsCount = element.GetNullableInt("descendants-count"),
                    CanUpdate = element.GetNullableBool("can-update"),
                    HasRead = element.GetNullableBool("has-read"),
                    Participation = element.Element("participation").ToParticipation(),
                    Attachment = element.Element("attachment")?.ToAttachement(),
                    Children =
                        (element.Element("children")?.Elements("child") ?? EmptyElementSet)
                            .Select(child => child.ToEntry())
                            .ToArray()
                };
        }

        public static Participation ToParticipation(this XElement element)
        {
            Assert.IsNotNull(element, nameof(element));

            return
                new Participation()
                {
                    Id = element.GetInt("id"),
                    Name = element.GetString("name"),
                    Group = element.Element("group")?.ToGroup(),
                    Admin = element.GetNullableBool("admin"),
                    CreatedAt = element.GetNullableDateTimeOffset("created-at"),
                    UpdatedAt = element.GetNullableDateTimeOffset("updated-at"),
                    Status = element.GetString("status")
                };
        }

        public static Group ToGroup(this XElement element)
        {
            Assert.IsNotNull(element, nameof(element));

            return
                new Group()
                {
                    Name = element.GetString("name"),
                    ToParam = element.GetInt("to-param"),
                    Id = element.GetNullableInt("id"),
                    CreatedAt = element.GetNullableDateTimeOffset("created-at"),
                    UpdatedAt = element.GetNullableDateTimeOffset("updated-at"),
                    Opened = element.GetNullableBool("opened"),
                    IsExpired = element.GetNullableBool("is-expired")
                };
        }

        public static Attachment ToAttachement(this XElement element)
        {
            Assert.IsNotNull(element, nameof(element));

            return
                new Attachment()
                {
                    ContentType = element.GetString("content-type"),
                    AttachementType = element.GetEnum<AttachmentType>("attachment-type"),
                    FileName = element.GetString("filename"),
                    OriginalFileName = element.GetString("original-filename"),
                    Url = element.Element("data-psych").GetUri("url")
                };
        }
    }
}
