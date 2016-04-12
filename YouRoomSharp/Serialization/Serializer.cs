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
                    Id = element.Element("id").AsInt(),
                    Content = element.Element("content").AsString(),
                    CreatedAt = element.Element("created-at").AsDateTimeOffset(),
                    UpdatedAt = element.Element("updated-at").AsDateTimeOffset(),
                    ParentId = element.Element("parent-id").AsNullableInt(),
                    DesdescendantsCount = element.Element("descendants-count").AsInt(),
                    RootId = element.Element("root-id").AsInt(),
                    CanUpdate = element.Element("can-update").AsBool(),
                    HasRead = element.Element("has-read").AsBool(),
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
                    Id = element.Element("id").AsInt(),
                    Name = element.Element("name").AsString(),
                    Group = element.Element("group").ToParticipationGroup()
                };
        }

        public static ParticipationGroup ToParticipationGroup(this XElement element)
        {
            Assert.IsNotNull(element, nameof(element));

            return
                new ParticipationGroup()
                {
                    Name = element.Element("name").AsString(),
                    ToParam = element.Element("to-param").AsInt()
                };
        }

        public static Group ToGroup(this XElement element)
        {
            Assert.IsNotNull(element, nameof(element));

            return
                new Group()
                {
                    Id = element.Element("id").AsInt(),
                    Name = element.Element("name").AsString(),
                    CreatedAt = element.Element("created-at").AsDateTimeOffset(),
                    UpdatedAt = element.Element("updated-at").AsDateTimeOffset(),
                    Opened = element.Element("opened").AsBool(),
                    ToParam = element.Element("to-param").AsInt(),
                    IsExpired = element.Element("is-expired").AsBool()
                };
        }

        public static Attachment ToAttachement(this XElement element)
        {
            Assert.IsNotNull(element, nameof(element));

            return
                new Attachment()
                {
                    ContentType = element.Element("content-type").AsNullableString(),
                    AttachementType = element.Element("attachment-type").AsEnum<AttachmentType>(),
                    FileName = element.Element("filename").AsNullableString(),
                    OriginalFileName = element.Element("original-filename").AsNullableString(),
                    Url = element.Element("data-psych").Element("url")?.AsUri()
                };
        }
    }
}
