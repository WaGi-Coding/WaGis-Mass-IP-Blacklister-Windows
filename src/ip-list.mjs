import { isIP } from 'node:net';

const IPV4_CANDIDATE =
  /(?<![\d.])(?:\d{1,3}\.){3}\d{1,3}(?:\/\d{1,2})?(?![\d.])/g;
const IPV6_CANDIDATE =
  /(?<![0-9a-f:])(?:[0-9a-f]{0,4}:){2,7}(?:(?:\d{1,3}\.){3}\d{1,3}|[0-9a-f]{0,4})(?:\/\d{1,3})?(?![0-9a-f:])/gi;

function isAddressOrCidr(value) {
  const [address, prefix, extra] = value.split('/');
  if (extra !== undefined) {
    return false;
  }

  const version = isIP(address);
  if (!version) {
    return false;
  }

  if (prefix === undefined) {
    return true;
  }

  const prefixLength = Number(prefix);
  const maximumPrefix = version === 4 ? 32 : 128;
  return (
    Number.isInteger(prefixLength) &&
    prefixLength >= 0 &&
    prefixLength <= maximumPrefix
  );
}

export function extractAddresses(text) {
  const source = String(text ?? '');
  const matches = [];

  for (const match of source.matchAll(IPV6_CANDIDATE)) {
    if (isAddressOrCidr(match[0])) {
      matches.push({
        value: match[0],
        start: match.index,
        end: match.index + match[0].length,
      });
    }
  }

  for (const match of source.matchAll(IPV4_CANDIDATE)) {
    if (!isAddressOrCidr(match[0])) {
      continue;
    }

    const start = match.index;
    const end = start + match[0].length;
    const partOfIpv6 = matches.some(
      (candidate) => start >= candidate.start && end <= candidate.end,
    );
    if (!partOfIpv6) {
      matches.push({ value: match[0], start, end });
    }
  }

  return matches
    .sort((left, right) => left.start - right.start)
    .map((match) => match.value);
}

export function applyAddressExclusions(addresses, exclusions) {
  const excluded = new Set(exclusions.map((address) => address.toLowerCase()));
  return addresses.filter((address) => !excluded.has(address.toLowerCase()));
}
